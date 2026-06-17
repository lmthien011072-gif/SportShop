using Microsoft.AspNetCore.Mvc;
using SportShop.Models;
using SportShop.Helpers;
using SportShop.Data.Repositories;
using System.Threading.Tasks;

namespace SportShop.Controllers
{
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private const string CartSessionKey = "ShoppingCart";

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private ShoppingCart GetCart()
        {
            return HttpContext.Session.Get<ShoppingCart>(CartSessionKey) ?? new ShoppingCart();
        }

        private void SaveCart(ShoppingCart cart)
        {
            HttpContext.Session.Set(CartSessionKey, cart);
        }

        // 1. Hiển thị trang Giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart.Items);
        }

        // 2. Thêm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> AddToCart(int variantId, int quantity, string returnUrl = null)
        {
            // FIX 1: Chặn ngay nếu ID <= 0 (Tránh lỗi Foreign Key khi thanh toán)
            if (variantId <= 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn đầy đủ Màu sắc và Kích thước trước khi thêm vào giỏ hàng.";
                return Redirect(returnUrl ?? "/");
            }

            var variant = await _unitOfWork.ProductVariant.GetFirstOrDefaultAsync(
                filter: v => v.Id == variantId,
                includeProperties: "Product"
            );

            // FIX 2: Không được NotFound() gây sập web
            if (variant == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm này không tồn tại hoặc đã bị xóa khỏi hệ thống.";
                return Redirect(returnUrl ?? "/");
            }

            var cart = GetCart();

            cart.AddItem(new CartItem
            {
                ProductId = variant.ProductId,
                VariantId = variant.Id,
                ProductName = variant.Product.Name,
                Size = variant.Size,
                Color = variant.Color,
                Price = variant.Product.Price,
                ThumbnailUrl = string.IsNullOrEmpty(variant.Product.ThumbnailUrl) ? "https://placehold.co/100x100" : variant.Product.ThumbnailUrl,
                Quantity = quantity > variant.StockQuantity ? variant.StockQuantity : quantity // Ngăn chặn mua lố tồn kho
            });

            SaveCart(cart);

            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";
            return RedirectToAction("Index");
        }

        // 3. Cập nhật số lượng
        [HttpPost]
        public IActionResult UpdateQuantity(int variantId, int newQuantity)
        {
            // Nếu người dùng cố tình F12 đổi HTML input thành số âm, ta xóa món hàng luôn
            if (newQuantity <= 0)
            {
                return Remove(variantId);
            }

            var cart = GetCart();
            cart.UpdateQuantity(variantId, newQuantity);
            SaveCart(cart);

            return RedirectToAction("Index");
        }

        // 4. Xóa món hàng
        public IActionResult Remove(int variantId)
        {
            var cart = GetCart();
            cart.RemoveItem(variantId);
            SaveCart(cart);

            return RedirectToAction("Index");
        }
    }
}