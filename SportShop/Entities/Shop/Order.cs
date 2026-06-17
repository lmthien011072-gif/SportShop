using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SportShop.Entities.Shop
{
    public class Order
    {
        [Key]
        public int Id { get; private set; }

        [Required]
        public int UserId { get; private set; }

        public DateTime OrderDate { get; private set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; private set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdvancePaymentAmount { get; private set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; private set; }

        [NotMapped]
        public decimal RemainingAmount => TotalAmount - PaidAmount;

        [Required]
        public string ShippingAddress { get; private set; }

        public bool IsCustomized { get; private set; }

        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }
        public OrderStatus Status { get; private set; }

        private readonly List<OrderItem> _orderItems = new List<OrderItem>();
        public virtual IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        protected Order() { }

        public Order(int userId, string shippingAddress, PaymentMethod paymentMethod, bool isCustomized = false)
        {
            if (string.IsNullOrWhiteSpace(shippingAddress))
                throw new ArgumentException("Địa chỉ giao hàng không được để trống.");

            if (isCustomized && paymentMethod == PaymentMethod.COD)
                throw new ArgumentException("Đơn hàng gia công hoặc mua số lượng lớn bắt buộc phải chuyển khoản đặt cọc trước.");

            UserId = userId;
            ShippingAddress = shippingAddress;
            PaymentMethod = paymentMethod;
            IsCustomized = isCustomized;
            OrderDate = DateTime.Now;
            Status = OrderStatus.Pending;

            PaymentStatus = paymentMethod == PaymentMethod.COD ? PaymentStatus.PendingCOD : PaymentStatus.Unpaid;
        }

        // ==========================================
        // 1. NGHIỆP VỤ (BEHAVIORS) VỀ SẢN PHẨM & TIỀN
        // ==========================================
        public void AddOrderItem(int productVariantId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0) throw new ArgumentException("Số lượng mua phải lớn hơn 0");
            _orderItems.Add(new OrderItem(productVariantId, quantity, unitPrice));
            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            TotalAmount = _orderItems.Sum(x => x.Quantity * x.UnitPrice);
        }

        public void SetAdvancePayment(decimal amount)
        {
            AdvancePaymentAmount = amount;
        }

        public void RecordPayment(decimal amount)
        {
            PaidAmount += amount;

            if (PaidAmount >= TotalAmount)
                PaymentStatus = PaymentStatus.Paid;
            else if (PaidAmount > 0)
                PaymentStatus = PaymentStatus.PartiallyPaid;
        }

        // ==========================================
        // 2. NGHIỆP VỤ (BEHAVIORS) VỀ TRẠNG THÁI GIAO HÀNG
        // ==========================================
        public void ConfirmOrder()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Chỉ có thể xác nhận đơn hàng đang ở trạng thái 'Chờ xác nhận'.");

            if (PaymentMethod == PaymentMethod.BankTransfer && PaymentStatus == PaymentStatus.Unpaid)
                throw new InvalidOperationException("Kế toán cần xác nhận đã nhận tiền (hoặc cọc) trước khi duyệt đơn chuyển khoản.");

            Status = OrderStatus.Confirmed;
        }

        public void MarkAsShipping()
        {
            if (Status != OrderStatus.Confirmed) throw new InvalidOperationException("Đơn hàng phải được 'Xác nhận' trước khi chuyển cho đơn vị vận chuyển.");
            Status = OrderStatus.Shipping;
        }

        public void MarkAsDelivered()
        {
            if (Status != OrderStatus.Shipping) throw new InvalidOperationException("Đơn hàng phải 'Đang giao' mới có thể chuyển sang 'Giao thành công'.");
            Status = OrderStatus.Delivered;

            if (PaymentMethod == PaymentMethod.COD && PaymentStatus == PaymentStatus.PendingCOD)
                PaymentStatus = PaymentStatus.Paid;
        }

        public void CancelOrder(bool isCustomerFault = false)
        {
            if (Status == OrderStatus.Shipping || Status == OrderStatus.Delivered || Status == OrderStatus.Returned)
                throw new InvalidOperationException("Không thể hủy đơn hàng đã xuất kho hoặc đang giao.");

            var previousStatus = Status;
            Status = OrderStatus.Cancelled;

            // Xử lý mất cọc hoặc hoàn tiền
            if (PaymentStatus == PaymentStatus.Paid || PaymentStatus == PaymentStatus.PartiallyPaid)
            {
                if (isCustomerFault)
                {
                    if (previousStatus == OrderStatus.Pending)
                    {
                        PaymentStatus = PaymentStatus.Refunded;
                    }
                    else if (previousStatus == OrderStatus.Confirmed)
                    {
                        if (IsCustomized)
                        {
                            if (PaymentStatus == PaymentStatus.PartiallyPaid)
                                PaymentStatus = PaymentStatus.DepositForfeited;
                            else if (PaymentStatus == PaymentStatus.Paid)
                                PaymentStatus = PaymentStatus.Refunded;
                        }
                        else
                        {
                            PaymentStatus = PaymentStatus.Refunded;
                        }
                    }
                }
                else
                {
                    PaymentStatus = PaymentStatus.Refunded;
                }
            }
        }

        public void ReturnOrder()
        {
            if (Status != OrderStatus.Shipping && Status != OrderStatus.Delivered)
                throw new InvalidOperationException("Chỉ có thể hoàn trả đơn hàng khi đang giao hoặc đã giao.");

            Status = OrderStatus.Returned;
            if (PaidAmount > 0) PaymentStatus = PaymentStatus.Refunded;
        }

        // ==========================================
        // 3. NGHIỆP VỤ (BEHAVIORS) VỀ THANH TOÁN (ADMIN)
        // ==========================================
        public void SwitchToCOD()
        {
            // Ràng buộc: Đơn gia công cấm chuyển sang COD
            if (IsCustomized)
                throw new InvalidOperationException("Đơn hàng Gia công / Mua số lượng lớn bắt buộc phải chuyển khoản cọc, không thể chuyển sang COD.");

            if (PaymentMethod == PaymentMethod.BankTransfer && PaymentStatus == PaymentStatus.Unpaid)
            {
                PaymentMethod = PaymentMethod.COD;
                PaymentStatus = PaymentStatus.PendingCOD;
                AdvancePaymentAmount = 0; // Đưa tiền cọc về 0 vì COD là thu 100% lúc nhận hàng
            }
            else
            {
                throw new InvalidOperationException("Không thể chuyển sang COD do sai trạng thái hiện tại.");
            }
        }
    }
}