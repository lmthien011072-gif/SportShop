using SportShop.ViewModels.Shared;

namespace SportShop.ViewModels.Home
{
    public class CategorySectionViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ProductCardViewModel> Products { get; set; } = new List<ProductCardViewModel>();
    }
}
