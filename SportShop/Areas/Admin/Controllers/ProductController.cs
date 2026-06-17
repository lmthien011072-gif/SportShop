using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportShop.Areas.Admin.ViewModels.Product;
using SportShop.Data.Repositories;
using SportShop.Entities.Shop;
using SportShop.Helpers;

namespace SportShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Product.GetAllAsync(includeProperties: "Category,Brand,Sport");
            var viewModel = _mapper.Map<IEnumerable<ProductAdminViewModel>>(products);
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ProductAdminViewModel();
            await PopulateDropdownsAsync(vm);
            // Khởi tạo sẵn 1 dòng biến thể trống để UI render form nhập liệu
            vm.Variants.Add(new ProductVariantAdminViewModel());
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductAdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            // 1. Ánh xạ thành Entity Product
            var product = _mapper.Map<Product>(model);

            // 2. Xử lý ảnh Thumbnail (Ảnh chính)
            if (model.ThumbnailFile != null)
            {
                string prefix = "prod_" + Guid.NewGuid().ToString("N").Substring(0, 5);
                product.ThumbnailUrl = await FileHelper.UploadImageAsync(model.ThumbnailFile, _env.WebRootPath, "products", prefix);
            }

            // 3. Xử lý các Biến thể (Variants)
            if (model.Variants != null && model.Variants.Any())
            {
                foreach (var v in model.Variants)
                {
                    product.ProductVariants.Add(new ProductVariant
                    {
                        Color = v.Color,
                        Size = v.Size,
                        StockQuantity = v.StockQuantity
                    });
                }
            }

            // 4. Lưu Product xuống DB để lấy ID tự tăng (Phục vụ cho Gallery)
            _unitOfWork.Product.AddAsync(product);
            await _unitOfWork.SaveAsync();

            // 5. Xử lý bộ ảnh Gallery (Nhiều ảnh)
            if (model.GalleryFiles != null && model.GalleryFiles.Any())
            {
                var galleryPaths = await FileHelper.UploadMultipleImagesAsync(model.GalleryFiles, _env.WebRootPath, "products/gallery", $"gal_{product.Id}");
                foreach (var path in galleryPaths)
                {
                    product.ProductImages.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = path
                        // ProductVariantId sẽ được thiết lập sau qua giao diện Edit nếu Admin muốn gắn ảnh cho từng màu
                    });
                }
                _unitOfWork.Product.Update(product);
                await _unitOfWork.SaveAsync();
            }

            TempData["Success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Product.GetFirstOrDefaultAsync(p => p.Id == id, includeProperties: "ProductImages");
            if (product != null)
            {
                // Dọn rác: Xóa ảnh chính
                if (!string.IsNullOrEmpty(product.ThumbnailUrl))
                    FileHelper.DeleteFile(_env.WebRootPath, product.ThumbnailUrl);

                // Dọn rác: Xóa ảnh gallery
                foreach (var img in product.ProductImages)
                {
                    FileHelper.DeleteFile(_env.WebRootPath, img.ImageUrl);
                }

                _unitOfWork.Product.Remove(product);
                await _unitOfWork.SaveAsync();
                TempData["Success"] = "Xóa sản phẩm thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync(ProductAdminViewModel vm)
        {
            vm.Categories = new SelectList(await _unitOfWork.Category.GetAllAsync(), "Id", "Name");
            vm.Brands = new SelectList(await _unitOfWork.Brand.GetAllAsync(), "Id", "Name");
            vm.Sports = new SelectList(await _unitOfWork.Sport.GetAllAsync(), "Id", "Name");
        }
    }
}