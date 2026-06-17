namespace SportShop.ViewModels.Order
{
    public class OrderItemViewModel
    {
        public string ProductName { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Price * Quantity;
    }
}
