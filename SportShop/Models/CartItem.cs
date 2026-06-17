using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SportShop.Entities.Shop;
using System.Text.Json.Serialization;

namespace SportShop.Models
{
    public class CartItem
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;

        [JsonIgnore]
        public decimal Total => Price * Quantity;
    }
}
