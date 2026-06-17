using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using SportShop.Entities.Shop;

namespace SportShop.Helpers
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole<int>>
    {
        public CustomClaimsPrincipalFactory(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            // Gọi hàm gốc để lấy các thông tin cơ bản (Id, UserName, Email...)
            var identity = await base.GenerateClaimsAsync(user);

            // ÉP KIỂU ENUM SANG CHUỖI: Chuyển UserRole.Admin thành chữ "Admin"
            var roleName = user.Role.ToString();

            // Thêm Claim Role vào Cookie
            identity.AddClaim(new Claim(ClaimTypes.Role, roleName));

            return identity;
        }
    }
}