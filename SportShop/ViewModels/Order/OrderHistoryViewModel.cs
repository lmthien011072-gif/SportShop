using System;
using SportShop.Entities;

namespace SportShop.ViewModels.Order
{
    public class OrderHistoryViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal AdvancePaymentAmount { get; set; }
        public bool IsCustomized { get; set; }

        public int TotalItemsCount { get; set; }
        public string FirstProductThumbnailUrl { get; set; }
        public string FirstProductName { get; set; }

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
            PaymentStatus.PartiallyPaid => "bg-warning text-dark",
            PaymentStatus.DepositForfeited => "bg-dark",
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
    }
}