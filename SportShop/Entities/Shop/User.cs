using Microsoft.AspNetCore.Identity;
using SportShop.Models;
using System.ComponentModel.DataAnnotations;

namespace SportShop.Entities.Shop
{
    public class User : IdentityUser<int>
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; } // Có thể lưu "Nam", "Nữ", "Khác"

        [MaxLength(500)]
        public string? AvatarUrl { get; set; } // Đường dẫn lưu ảnh đại diện

        public UserRole Role { get; set; } = UserRole.Customer;

        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        // Mối quan hệ 1 - Nhiều: Một User có một sổ địa chỉ
        public virtual ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    }
}
