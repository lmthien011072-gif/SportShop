using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportShop.Areas.Admin.ViewModels;
using SportShop.Areas.Admin.ViewModels.Order;
using SportShop.Data.Repositories;
using SportShop.Entities;
using SportShop.Entities.Shop; // Cần thêm để nhận diện OrderStatus
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Danh sách đơn hàng
        public async Task<IActionResult> Index()
        {
            var orders = await _unitOfWork.Order.GetAllAsync();
            var viewModel = _mapper.Map<IEnumerable<OrderListViewModel>>(orders.OrderByDescending(o => o.OrderDate));

            foreach (var item in viewModel)
            {
                var user = await _unitOfWork.User.GetByIdAsync(int.Parse(item.CustomerName.Replace("Khách hàng #", "")));
                if (user != null) item.CustomerName = user.FullName;
            }

            return View(viewModel);
        }

        // Chi tiết 1 đơn hàng
        public async Task<IActionResult> Details(int id)
        {
            var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(
                filter: o => o.Id == id,
                includeProperties: "OrderItems,OrderItems.ProductVariant,OrderItems.ProductVariant.Product"
            );

            if (order == null) return NotFound();

            var viewModel = _mapper.Map<OrderDetailAdminViewModel>(order);

            var user = await _unitOfWork.User.GetByIdAsync(order.UserId);
            if (user != null)
            {
                viewModel.CustomerName = user.FullName;
                viewModel.PhoneNumber = user.PhoneNumber;
            }

            return View(viewModel);
        }

        // Cập nhật trạng thái giao hàng
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int id, string actionType)
        {
            var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(
                filter: o => o.Id == id,
                includeProperties: "OrderItems,OrderItems.ProductVariant"
            );
            if (order == null) return NotFound();

            // Nếu Admin bấm HỦY ĐƠN (CANCEL), cần bọc bằng Transaction để vừa Hủy đơn vừa Hoàn kho an toàn
            if (actionType == "CANCEL")
            {
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    // Truyền false để ám chỉ "Lỗi do Shop hủy", hoặc tùy logic của bạn
                    order.CancelOrder(isCustomerFault: false);

                    // Hoàn trả lại số lượng (Stock) vào kho
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

                    TempData["Success"] = "Đã hủy đơn hàng và hoàn lại số lượng sản phẩm vào kho.";
                }
                catch (System.Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "Lỗi khi hủy đơn: " + ex.Message;
                }
                return RedirectToAction(nameof(Details), new { id = id });
            }

            // Xử lý các trạng thái bình thường (Duyệt, Giao, Hoàn thành)
            try
            {
                switch (actionType)
                {
                    case "CONFIRM": order.ConfirmOrder(); break;
                    case "SHIP": order.MarkAsShipping(); break;
                    case "DELIVER": order.MarkAsDelivered(); break;
                    case "RETURN": order.ReturnOrder(); break;
                }

                _unitOfWork.Order.Update(order);
                await _unitOfWork.SaveAsync();
                TempData["Success"] = "Cập nhật tiến trình đơn hàng thành công!";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = "Thao tác thất bại: " + ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        // Kế toán nhập số tiền nhận được (Tiền cọc hoặc trả đủ)
        [HttpPost]
        public async Task<IActionResult> RecordDeposit(int orderId, decimal amount)
        {
            var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return NotFound();

            order.RecordPayment(amount);

            _unitOfWork.Order.Update(order);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = $"Đã ghi nhận số tiền {amount.ToString("N0")}đ vào đơn hàng!";
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        // Admin chuyển đơn sang COD khi gọi điện chốt với khách
        [HttpPost]
        public async Task<IActionResult> SwitchToCOD(int id)
        {
            var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            try
            {
                order.SwitchToCOD(); // Entity Order sẽ chặn lại nếu đây là Đơn Gia công
                _unitOfWork.Order.Update(order);
                await _unitOfWork.SaveAsync();
                TempData["Success"] = "Đã chuyển phương thức thanh toán sang Thu tiền mặt (COD) thành công!";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message; // Hiển thị lỗi từ Entity ném ra
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}