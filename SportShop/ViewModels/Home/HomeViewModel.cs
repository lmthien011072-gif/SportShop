using SportShop.ViewModels.Shared;

namespace SportShop.ViewModels.Home
{
    // Class tổng gom tất cả dữ liệu ném ra cho file Index.cshtml của Home
    public class HomeViewModel
    {
        // Danh sách sản phẩm hot (Sản phẩm nổi bật)
        public List<ProductCardViewModel> FeaturedProducts { get; set; } = new List<ProductCardViewModel>();

        // Danh sách các danh mục kèm theo sản phẩm bên trong
        public List<CategorySectionViewModel> CategorySections { get; set; } = new List<CategorySectionViewModel>();
    }
}