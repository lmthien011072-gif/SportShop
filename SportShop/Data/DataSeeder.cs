using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SportShop.Entities; // FIX: Để lấy Enum UserRole
using SportShop.Entities.Shop;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SportShop.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            string adminEmail = "admin@sportshop.com";
            string adminPassword = "Admin@123456";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Quản Trị Viên Hệ Thống",
                    EmailConfirmed = true,
                    Role = UserRole.Admin,
                    AvatarUrl = "",
                    Gender = "Khác"
                };

                var createPowerUser = await userManager.CreateAsync(newAdmin, adminPassword);

                if (!createPowerUser.Succeeded)
                {
                    // In thẳng các lỗi ra console để debug (VD: Mật khẩu yếu, Email không hợp lệ...)
                    var errors = string.Join(", ", createPowerUser.Errors.Select(e => e.Description));
                    throw new Exception($"Không thể tạo tài khoản Admin. Chi tiết lỗi: {errors}");
                }
            }
            else
            {
                // Nếu tài khoản đã tồn tại mà chưa được cấp quyền Admin thì cấp lại
                if (adminUser.Role != UserRole.Admin)
                {
                    adminUser.Role = UserRole.Admin;
                    await userManager.UpdateAsync(adminUser);
                }
            }
        }
    }
}