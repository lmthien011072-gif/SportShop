using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SportShop.Areas.Admin.ViewModels.Product
{
    public class ProductAdminViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        public string? ThumbnailUrl { get; set; }

        [Display(Name = "Ảnh đại diện (Thumbnail)")]
        public IFormFile? ThumbnailFile { get; set; }

        public string? Description { get; set; }

        // Khóa ngoại
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thương hiệu")]
        public int BrandId { get; set; }
        public string? BrandName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn môn thể thao")]
        public int SportId { get; set; }
        public string? SportName { get; set; }

        // Nạp dữ liệu cho Dropdown
        public IEnumerable<SelectListItem>? Categories { get; set; }
        public IEnumerable<SelectListItem>? Brands { get; set; }
        public IEnumerable<SelectListItem>? Sports { get; set; }

        // Danh sách biến thể và hình ảnh
        public List<ProductVariantAdminViewModel> Variants { get; set; } = new List<ProductVariantAdminViewModel>();
        public List<ProductImageAdminViewModel> Images { get; set; } = new List<ProductImageAdminViewModel>();

        [Display(Name = "Thêm ảnh Gallery")]
        public List<IFormFile>? GalleryFiles { get; set; }
    }
}
