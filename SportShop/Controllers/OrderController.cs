using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportShop.Data.Repositories;
using SportShop.Entities.Shop;
using SportShop.Entities;
using SportShop.Models;
using SportShop.Services;
using SportShop.ViewModels.Order;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SportShop.Helpers;

namespace SportShop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(UserManager<User> userManager, IUnitOfWork unitOfWork, IOrderService orderService, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.Get<ShoppingCart>("ShoppingCart");
            if (cart == null || !cart.Items.Any()) return RedirectToAction("Index", "Cart");

            var user = await _userManager.GetUserAsync(User);
            var userWithAddresses = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.Id == user.Id, includeProperties: "Addresses");

            var savedAddresses = userWithAddresses?.Addresses.ToList() ?? new List<UserAddress>();

            var model = new CheckoutViewModel
            {
                CartItems = cart.Items,
                TotalAmount = cart.TotalPrice,
                SavedAddresses = savedAddresses,
                AddressType = savedAddresses.Any() ? "SAVED" : "NEW",
                PaymentMethod = PaymentMethod.COD
            };

            var defaultAddress = savedAddresses.FirstOrDefault(a => a.IsDefault);
            if (defaultAddress != null)
            {
                model.SelectedAddressId = defaultAddress.Id;
            }
            else if (savedAddresses.Any())
            {
                model.SelectedAddressId = savedAddresses.First().Id;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = HttpContext.Session.Get<ShoppingCart>("ShoppingCart");
            if (cart == null || !cart.Items.Any()) return RedirectToAction("Index", "Cart");

            var user = await _userManager.GetUserAsync(User);
            string finalShippingAddress = "";

            if (model.AddressType == "SAVED")
            {
                var keysToRemove = ModelState.Keys.Where(k => k.StartsWith(nameof(model.NewAddress))).ToList();
                foreach (var key in keysToRemove)
                {
                    ModelState.Remove(key);
                }

                if (!model.SelectedAddressId.HasValue || model.SelectedAddressId.Value <= 0)
                {
                    ModelState.AddModelError("SelectedAddressId", "Vui lòng chọn một địa chỉ giao hàng có sẵn.");
                    return await ReloadCheckoutView(model, cart.Items, user);
                }

                var userWithAddresses = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.Id == user.Id, includeProperties: "Addresses");
                var addr = userWithAddresses?.Addresses.FirstOrDefault(a => a.Id == model.SelectedAddressId.Value);

                if (addr == null)
                {
                    ModelState.AddModelError("", "Địa chỉ đã chọn không hợp lệ hoặc đã bị xóa trước đó.");
                    return await ReloadCheckoutView(model, cart.Items, user);
                }

                finalShippingAddress = $"Người nhận: {addr.ReceiverName} | SĐT: {addr.PhoneNumber} | Đ/c: {addr.SpecificAddress}, {addr.Commune}, {addr.Province}";
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Vui lòng điền đầy đủ tất cả thông tin hợp lệ của địa chỉ giao hàng mới.");
                    return await ReloadCheckoutView(model, cart.Items, user);
                }

                finalShippingAddress = $"Người nhận: {model.NewAddress.ReceiverName} | SĐT: {model.NewAddress.PhoneNumber} | Đ/c: {model.NewAddress.SpecificAddress}, {model.NewAddress.Commune}, {model.NewAddress.Province}";

                var userWithAddresses = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.Id == user.Id, includeProperties: "Addresses");
                if (userWithAddresses != null)
                {
                    bool isFirstAddress = !userWithAddresses.Addresses.Any();
                    var newAddress = new UserAddress
                    {
                        ReceiverName = model.NewAddress.ReceiverName,
                        PhoneNumber = model.NewAddress.PhoneNumber,
                        Province = model.NewAddress.Province,
                        Commune = model.NewAddress.Commune,
                        SpecificAddress = model.NewAddress.SpecificAddress,
                        IsDefault = isFirstAddress
                    };
                    userWithAddresses.Addresses.Add(newAddress);
                }
            }

            if (model.IsCustomized && model.PaymentMethod == PaymentMethod.COD)
            {
                ModelState.AddModelError("", "Đơn hàng Yêu cầu gia công hoặc Số lượng lớn bắt buộc phải Chuyển khoản thanh toán / đặt cọc.");
                return await ReloadCheckoutView(model, cart.Items, user);
            }

            var result = await _orderService.PlaceOrderAsync(
                user.Id,
                cart,
                finalShippingAddress,
                model.PaymentMethod,   // Đã map tự động sang Enum
                model.IsCustomized,    // Đơn gia công/Sỉ
                model.IsPartialPayment // Có chọn đặt cọc hay không
            );

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return await ReloadCheckoutView(model, cart.Items, user);
            }

            HttpContext.Session.Remove("ShoppingCart");

            if (model.PaymentMethod == PaymentMethod.BankTransfer)
            {
                return RedirectToAction("QrPayment", "Order", new { orderId = result.OrderId });
            }

            return RedirectToAction("Success", new { orderId = result.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> Success(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(o => o.Id == orderId && o.UserId == user.Id);
            if (order == null) return NotFound();

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> QrPayment(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(o => o.Id == orderId && o.UserId == user.Id);
            if (order == null) return NotFound();

            decimal amountToPay = order.AdvancePaymentAmount > 0 ? order.AdvancePaymentAmount : order.TotalAmount;

            ViewBag.OrderId = orderId;
            ViewBag.AmountToPay = amountToPay;
            ViewBag.IsPartial = order.AdvancePaymentAmount > 0 && order.AdvancePaymentAmount < order.TotalAmount;

            return View("QrPayment");
        }

        private async Task<IActionResult> ReloadCheckoutView(CheckoutViewModel model, List<CartItem> cartItems, User user)
        {
            model.CartItems = cartItems;
            model.TotalAmount = cartItems.Sum(c => c.Total);
            var refreshAddrs = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.Id == user.Id, includeProperties: "Addresses");
            model.SavedAddresses = refreshAddrs?.Addresses.ToList() ?? new List<UserAddress>();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);

            var orders = await _unitOfWork.Order.GetAllAsync(filter: o => o.UserId == user.Id);
            var sortedOrders = orders.OrderByDescending(o => o.OrderDate).ToList();

            var viewModel = _mapper.Map<List<OrderHistoryViewModel>>(sortedOrders);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(
                filter: o => o.Id == id && o.UserId == user.Id,
                includeProperties: "OrderItems,OrderItems.ProductVariant,OrderItems.ProductVariant.Product"
            );

            if (order == null) return NotFound();

            var viewModel = _mapper.Map<OrderDetailViewModel>(order);

            return View(viewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelMyOrder(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);

            var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(
                filter: o => o.Id == orderId && o.UserId == user.Id,
                includeProperties: "OrderItems,OrderItems.ProductVariant"
            );

            if (order != null && (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Confirmed))
            {
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    // 1. Chuyển trạng thái đơn và xử lý tiền (Sẽ mất cọc nếu đã Confirmed và là Đơn Gia công)
                    order.CancelOrder(isCustomerFault: true);

                    // 2. Hoàn lại số lượng vào kho (Stock)
                    foreach (var item in order.OrderItems)
                    {
                        if (item.ProductVariant != null)
                        {
                            item.ProductVariant.StockQuantity += item.Quantity;
                            _unitOfWork.ProductVariant.Update(item.ProductVariant);
                        }
                    }

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "Bạn đã hủy đơn hàng thành công!";
                }
                catch
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "Có lỗi xảy ra khi hủy đơn hàng. Vui lòng thử lại.";
                }
            }
            else
            {
                TempData["Error"] = "Đơn hàng đã xuất kho hoặc không thể hủy vào lúc này. Vui lòng liên hệ Hotline.";
            }

            return RedirectToAction(nameof(Details), new { id = orderId });
        }
    }
}