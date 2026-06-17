using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Areas.Admin.ViewModels.User;
using SportShop.Entities; // Gọi Enum
using SportShop.Entities.Shop;

namespace SportShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userVMs = new List<UserAdminViewModel>();

            foreach (var u in users)
            {
                userVMs.Add(new UserAdminViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber ?? "N/A",
                    Role = u.Role,
                    IsLockedOut = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.Now
                });
            }

            return View(userVMs);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLock(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            if (user.Role == UserRole.Admin)
            {
                TempData["Error"] = "Không thể khóa tài khoản Quản trị viên!";
                return RedirectToAction(nameof(Index));
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now)
            {
                // Đang khóa -> Mở khóa
                user.LockoutEnd = null;
                TempData["Success"] = $"Đã mở khóa tài khoản {user.Email}";
            }
            else
            {
                // Đang mở -> Khóa 100 năm
                user.LockoutEnd = DateTimeOffset.Now.AddYears(100);
                TempData["Success"] = $"Đã khóa tài khoản {user.Email}";
            }

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}