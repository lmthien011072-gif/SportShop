using SportShop.ViewModels.Shared;
using System.Collections.Generic;

namespace SportShop.ViewModels.Product
{
    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; } // Thêm CategoryId cho Breadcrumb
        public string CategoryName { get; set; }

        public int BrandId { get; set; } // Thêm BrandId
        public string BrandName { get; set; }

        public int SportId { get; set; } // Thêm SportId
        public string SportName { get; set; }

        public string MainImageUrl { get; set; }
        public List<string> GalleryImages { get; set; } = new List<string>();
        public List<VariantOptionViewModel> AvailableVariants { get; set; } = new List<VariantOptionViewModel>();

        // Thêm danh sách sản phẩm liên quan
        public List<ProductCardViewModel> RelatedProducts { get; set; } = new List<ProductCardViewModel>();
    }
}