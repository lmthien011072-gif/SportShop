using System.ComponentModel.DataAnnotations;

namespace SportShop.Areas.Admin.ViewModels.Catalog
{
    public class CategoryAdminViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
