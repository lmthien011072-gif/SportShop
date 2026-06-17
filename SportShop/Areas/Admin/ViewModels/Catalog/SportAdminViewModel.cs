using System.ComponentModel.DataAnnotations;

namespace SportShop.Areas.Admin.ViewModels.Catalog
{
    public class SportAdminViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên môn thể thao")]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
