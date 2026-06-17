using System.ComponentModel.DataAnnotations;

namespace SportShop.Entities.Location
{
    public class Commune
    {
        [Key]
        [MaxLength(10)]
        public string Code { get; set; } // Mã xã

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(10)]
        public string ProvinceCode { get; set; } // Khóa ngoại trỏ về Tỉnh

        public virtual Province Province { get; set; }
    }
}
