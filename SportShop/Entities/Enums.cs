namespace SportShop.Entities
{
    public enum UserRole : byte
    {
        Customer = 0,
        Admin = 1
    }

    public enum OrderStatus : byte
    {
        Pending = 0,        // 1. Chờ xác nhận (Khách vừa đặt xong)
        Confirmed = 1,      // 2. Đã xác nhận / Đang chuẩn bị hàng
        Shipping = 2,       // 3. Đang giao hàng (Đã đưa cho Shipper)
        Delivered = 3,      // 4. Giao hàng thành công
        Cancelled = 4,      // 5. Đã hủy (Bởi khách hoặc Admin)
        Returned = 5        // 6. Trả hàng / Hoàn tiền
    }

    public enum PaymentStatus : byte
    {
        Unpaid = 0,         // 1. Chưa thanh toán
        Paid = 1,           // 2. Đã thanh toán
        PendingCOD = 2,     // 3. Chờ thu hộ (Khách chọn COD, shipper sẽ thu)
        Failed = 3,         // 4. Lỗi giao dịch (Ngân hàng từ chối...)
        Refunded = 4,        // 5. Đã hoàn tiền (Trường hợp hủy đơn đã thanh toán)
        PartiallyPaid = 5,     // 6. Đã đặt cọc (do khách 
        DepositForfeited = 6   // 7. Mất cọc
    }

    public enum PaymentMethod : byte
    {
        COD = 0,            // 1. Thanh toán khi nhận hàng
        BankTransfer = 1,   // 2. Chuyển khoản ngân hàng (Quét mã QR)
        EWallet = 2         // 3. Ví điện tử (Momo, ZaloPay...)
    }
}