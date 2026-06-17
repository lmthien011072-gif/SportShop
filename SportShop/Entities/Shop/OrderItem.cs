using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SportShop.Entities.Shop
{
    public class OrderItem
    {
        [Key]
        public int Id { get; private set; }

        [Required]
        public int OrderId { get; private set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; private set; }

        [Required]
        public int ProductVariantId { get; private set; }

        [ForeignKey("ProductVariantId")]
        public virtual ProductVariant ProductVariant { get; private set; }

        [Required]
        public int Quantity { get; private set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; private set; }

        protected OrderItem() { }

        // Constructor có internal để chỉ riêng Class Order mới có quyền khởi tạo nó
        public OrderItem(int productVariantId, int quantity, decimal unitPrice)
        {
            ProductVariantId = productVariantId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
