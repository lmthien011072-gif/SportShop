using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SportShop.Entities.Shop;
using System.Text.Json.Serialization;

namespace SportShop.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        [JsonIgnore]
        public int TotalItems => Items.Sum(x => x.Quantity);

        [JsonIgnore]
        public decimal TotalPrice => Items.Sum(x => x.Total);

        public void AddItem(CartItem item)
        {
            var existing = Items.FirstOrDefault(x => x.VariantId == item.VariantId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }

        public void RemoveItem(int variantId) => Items.RemoveAll(x => x.VariantId == variantId);

        public void UpdateQuantity(int variantId, int quantity)
        {
            var item = Items.FirstOrDefault(x => x.VariantId == variantId);
            if (item != null)
            {
                item.Quantity = quantity > 0 ? quantity : 1; // Ngăn chặn việc truyền số âm
            }
        }

        public void Clear() => Items.Clear();
    }
}
