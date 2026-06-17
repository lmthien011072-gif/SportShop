using System.ComponentModel.DataAnnotations;

namespace SportShop.Areas.Admin.ViewModels.Catalog
{
    public class BrandAdminViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên thương hiệu")]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? LogoUrl { get; set; }

        [Display(Name = "Tải Logo mới")]
        public IFormFile? LogoFile { get; set; }
    }
}
