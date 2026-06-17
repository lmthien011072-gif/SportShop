namespace SportShop.Areas.Admin.ViewModels.Order
{
    public class OrderItemAdminViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}
