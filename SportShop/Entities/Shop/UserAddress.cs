using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SportShop.Entities.Shop
{
    public class UserAddress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        [MaxLength(100)]
        public string ReceiverName { get; set; } // Tên người nhận

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string SpecificAddress { get; set; } // Số nhà, tên đường, tòa nhà...

        [Required]
        [MaxLength(100)]
        public string Province { get; set; }        // Tỉnh / Thành phố

        [Required]
        [MaxLength(100)]
        public string Commune { get; set; }         // Xã / Phường / Đặc khu

        // Đánh dấu đây là địa chỉ mặc định
        public bool IsDefault { get; set; }
    }
}
