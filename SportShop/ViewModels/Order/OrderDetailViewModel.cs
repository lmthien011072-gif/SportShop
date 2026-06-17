using SportShop.Entities;

namespace SportShop.ViewModels.Order
{
    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingAddress { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal AdvancePaymentAmount { get; set; } // Số tiền cọc
        public decimal PaidAmount { get; set; }           // Đã thanh toán thực tế
        public decimal RemainingAmount { get; set; }      // Còn lại phải thu (COD)
        public bool IsCustomized { get; set; }            // Đơn gia công/Sỉ

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public string PaymentMethodName => PaymentMethod switch
        {
            PaymentMethod.COD => "Thanh toán khi nhận hàng",
            PaymentMethod.BankTransfer => "Chuyển khoản / Quét mã",
            PaymentMethod.EWallet => "Ví điện tử",
            _ => "Không xác định"
        };

        public string PaymentStatusName => PaymentStatus switch
        {
            PaymentStatus.Unpaid => "Chưa thanh toán",
            PaymentStatus.Paid => "Đã thanh toán",
            PaymentStatus.PendingCOD => "Chờ thu hộ",
            PaymentStatus.Failed => "Lỗi giao dịch",
            PaymentStatus.Refunded => "Đã hoàn tiền",
            PaymentStatus.PartiallyPaid => "Đã đặt cọc 30%",
            PaymentStatus.DepositForfeited => "Mất cọc",
            _ => "Không xác định"
        };

        public string OrderStatusName => OrderStatus switch
        {
            OrderStatus.Pending => "Chờ duyệt",
            OrderStatus.Confirmed => "Đã xác nhận",
            OrderStatus.Shipping => "Đang giao hàng",
            OrderStatus.Delivered => "Hoàn thành",
            OrderStatus.Cancelled => "Đã hủy",
            OrderStatus.Returned => "Trả hàng",
            _ => "Không xác định"
        };

        public string PaymentStatusBadgeClass => PaymentStatus switch
        {
            PaymentStatus.Paid => "bg-success",
            PaymentStatus.Unpaid => "bg-danger",
            PaymentStatus.PendingCOD => "bg-secondary",
            PaymentStatus.Refunded => "bg-info text-dark",
            PaymentStatus.PartiallyPaid => "bg-warning text-dark", // Màu vàng cho đặt cọc
            PaymentStatus.DepositForfeited => "bg-dark",           // Màu đen cho mất cọc
            _ => "bg-light text-dark"
        };

        public string OrderStatusBadgeClass => OrderStatus switch
        {
            OrderStatus.Pending => "bg-warning text-dark",
            OrderStatus.Confirmed => "bg-primary",
            OrderStatus.Shipping => "bg-info text-dark",
            OrderStatus.Delivered => "bg-success",
            OrderStatus.Cancelled => "bg-danger",
            OrderStatus.Returned => "bg-secondary",
            _ => "bg-light text-dark"
        };

        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }
}
