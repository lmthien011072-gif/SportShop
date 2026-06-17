using SportShop.Entities;

namespace SportShop.Areas.Admin.ViewModels.Order
{
    public class OrderDetailAdminViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AdvancePaymentAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; } // Chỉ để view hiển thị, mapping từ entity
        public bool IsCustomized { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItemAdminViewModel> Items { get; set; } = new List<OrderItemAdminViewModel>();
    }
}
