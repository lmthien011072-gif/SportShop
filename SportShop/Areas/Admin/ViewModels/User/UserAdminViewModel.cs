using SportShop.Entities;

namespace SportShop.Areas.Admin.ViewModels.User
{
    public class UserAdminViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public UserRole Role { get; set; }
        public bool IsLockedOut { get; set; }
    }
}
