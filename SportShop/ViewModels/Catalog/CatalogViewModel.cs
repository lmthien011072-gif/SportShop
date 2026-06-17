using SportShop.ViewModels.Menu;
using SportShop.ViewModels.Shared;
using System.Collections.Generic;

namespace SportShop.ViewModels.Catalog
{
    public class CatalogViewModel
    {
        // --- TRẠNG THÁI BỘ LỌC HIỆN TẠI ---
        public int? ActiveCategoryId { get; set; }
        public string ActiveCategoryName { get; set; }

        public int? ActiveSportId { get; set; }
        public string ActiveSportName { get; set; }

        public int? ActiveBrandId { get; set; }
        public string ActiveBrandName { get; set; }
        public string ActiveBrandLogoUrl { get; set; } // Dành riêng cho Banner Brand

        // --- DANH SÁCH BỘ LỌC TƯƠNG TÁC (Tự động cập nhật dựa trên kết quả) ---
        public List<CategoryMenuItem> AvailableCategories { get; set; } = new List<CategoryMenuItem>();
        public List<SportMenuItem> AvailableSports { get; set; } = new List<SportMenuItem>();
        public List<BrandMenuItem> AvailableBrands { get; set; } = new List<BrandMenuItem>();

        // --- DANH SÁCH SẢN PHẨM HIỂN THỊ ---
        public List<ProductCardViewModel> Products { get; set; } = new List<ProductCardViewModel>();
    }
}