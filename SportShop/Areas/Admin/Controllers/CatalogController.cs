using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SportShop.Areas.Admin.ViewModels;
using SportShop.Areas.Admin.ViewModels.Catalog;
using SportShop.Data.Repositories;
using SportShop.Entities.Shop;
using SportShop.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CatalogController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public CatalogController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
        }

        // ==========================================
        // 1. QUẢN LÝ THƯƠNG HIỆU (BRAND) CÓ UPLOAD ẢNH
        // ==========================================
        public async Task<IActionResult> Brands()
        {
            var brands = await _unitOfWork.Brand.GetAllAsync();
            return View("Brands", _mapper.Map<IEnumerable<BrandAdminViewModel>>(brands));
        }

        [HttpPost]
        public async Task<IActionResult> SaveBrand(BrandAdminViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Dữ liệu không hợp lệ");

            Brand brand;
            if (model.Id == 0) // Thêm mới
            {
                brand = new Brand { Name = model.Name };

                // Upload Logo
                if (model.LogoFile != null)
                {
                    string prefix = "brand_" + Guid.NewGuid().ToString("N").Substring(0, 5);
                    brand.LogoUrl = await FileHelper.UploadImageAsync(model.LogoFile, _env.WebRootPath, "brands", prefix);
                }

                await _unitOfWork.Brand.AddAsync(brand); // ĐÃ SỬA THÀNH AddAsync
                TempData["Success"] = "Thêm thương hiệu thành công!";
            }
            else // Cập nhật
            {
                brand = await _unitOfWork.Brand.GetFirstOrDefaultAsync(b => b.Id == model.Id);
                if (brand == null) return NotFound();

                brand.Name = model.Name;

                // Xử lý đổi Logo
                if (model.LogoFile != null)
                {
                    // Xóa ảnh cũ
                    if (!string.IsNullOrEmpty(brand.LogoUrl))
                        FileHelper.DeleteFile(_env.WebRootPath, brand.LogoUrl);

                    // Up ảnh mới
                    string prefix = "brand_" + Guid.NewGuid().ToString("N").Substring(0, 5);
                    brand.LogoUrl = await FileHelper.UploadImageAsync(model.LogoFile, _env.WebRootPath, "brands", prefix);
                }

                _unitOfWork.Brand.Update(brand);
                TempData["Success"] = "Cập nhật thương hiệu thành công!";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Brands));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _unitOfWork.Brand.GetFirstOrDefaultAsync(b => b.Id == id);
            if (brand != null)
            {
                if (!string.IsNullOrEmpty(brand.LogoUrl))
                    FileHelper.DeleteFile(_env.WebRootPath, brand.LogoUrl);

                _unitOfWork.Brand.Remove(brand);
                await _unitOfWork.SaveAsync();
                TempData["Success"] = "Đã xóa thương hiệu!";
            }
            return RedirectToAction(nameof(Brands));
        }

        // ==========================================
        // 2. QUẢN LÝ DANH MỤC (CATEGORY)
        // ==========================================
        public async Task<IActionResult> Categories()
        {
            var categories = await _unitOfWork.Category.GetAllAsync();
            return View("Categories", _mapper.Map<IEnumerable<CategoryAdminViewModel>>(categories));
        }

        [HttpPost]
        public async Task<IActionResult> SaveCategory(CategoryAdminViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Dữ liệu không hợp lệ");

            if (model.Id == 0)
            {
                var category = new Category { Name = model.Name };
                await _unitOfWork.Category.AddAsync(category); // ĐÃ SỬA THÀNH AddAsync
                TempData["Success"] = "Thêm danh mục thành công!";
            }
            else
            {
                var category = await _unitOfWork.Category.GetFirstOrDefaultAsync(c => c.Id == model.Id);
                if (category == null) return NotFound();

                category.Name = model.Name;
                _unitOfWork.Category.Update(category);
                TempData["Success"] = "Cập nhật danh mục thành công!";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Category.GetFirstOrDefaultAsync(c => c.Id == id);
            if (category != null)
            {
                _unitOfWork.Category.Remove(category);
                await _unitOfWork.SaveAsync();
                TempData["Success"] = "Đã xóa danh mục!";
            }
            return RedirectToAction(nameof(Categories));
        }

        // ==========================================
        // 3. QUẢN LÝ MÔN THỂ THAO (SPORT)
        // ==========================================
        public async Task<IActionResult> Sports()
        {
            var sports = await _unitOfWork.Sport.GetAllAsync();
            return View("Sports", _mapper.Map<IEnumerable<SportAdminViewModel>>(sports));
        }

        [HttpPost]
        public async Task<IActionResult> SaveSport(SportAdminViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Dữ liệu không hợp lệ");

            if (model.Id == 0)
            {
                var sport = new Sport { Name = model.Name };
                await _unitOfWork.Sport.AddAsync(sport); // ĐÃ SỬA THÀNH AddAsync
                TempData["Success"] = "Thêm môn thể thao thành công!";
            }
            else
            {
                var sport = await _unitOfWork.Sport.GetFirstOrDefaultAsync(s => s.Id == model.Id);
                if (sport == null) return NotFound();

                sport.Name = model.Name;
                _unitOfWork.Sport.Update(sport);
                TempData["Success"] = "Cập nhật môn thể thao thành công!";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Sports));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSport(int id)
        {
            var sport = await _unitOfWork.Sport.GetFirstOrDefaultAsync(s => s.Id == id);
            if (sport != null)
            {
                _unitOfWork.Sport.Remove(sport);
                await _unitOfWork.SaveAsync();
                TempData["Success"] = "Đã xóa môn thể thao!";
            }
            return RedirectToAction(nameof(Sports));
        }
    }
}