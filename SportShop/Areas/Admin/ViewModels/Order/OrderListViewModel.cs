using SportShop.Entities;

namespace SportShop.Areas.Admin.ViewModels.Order
{
    public class OrderListViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AdvancePaymentAmount { get; set; }
        public bool IsCustomized { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus Status { get; set; }
    }
}
