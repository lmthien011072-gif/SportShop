using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportShop.Entities.Shop;
using SportShop.Helpers;
using SportShop.ViewModels.User;
using System;
using System.Threading.Tasks;

namespace SportShop.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(UserManager<User> userManager, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var viewModel = _mapper.Map<UserProfileViewModel>(user);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;

            // XỬ LÝ ẢNH ĐẠI DIỆN
            try
            {
                if (model.AvatarFile != null)
                {
                    if (!string.IsNullOrEmpty(user.AvatarUrl))
                    {
                        FileHelper.DeleteFile(_webHostEnvironment.WebRootPath, user.AvatarUrl);
                    }

                    string prefix = $"avatar_user{user.Id}";
                    string relativePath = await FileHelper.UploadImageAsync(model.AvatarFile, _webHostEnvironment.WebRootPath, "avatars", prefix);

                    if (!string.IsNullOrEmpty(relativePath))
                    {
                        user.AvatarUrl = relativePath;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("AvatarFile", ex.Message);
                return View("Index", model);
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Cập nhật thông tin thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật.";
            }

            return RedirectToAction("Index");
        }
    }
}