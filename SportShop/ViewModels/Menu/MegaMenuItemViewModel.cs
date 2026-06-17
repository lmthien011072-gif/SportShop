namespace SportShop.ViewModels.Menu
{
    public class MegaMenuItemViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string IconClass { get; set; }
        public List<SportMenuItem> RelatedSports { get; set; } = new List<SportMenuItem>();
        public List<BrandMenuItem> RelatedBrands { get; set; } = new List<BrandMenuItem>();
    }
}
