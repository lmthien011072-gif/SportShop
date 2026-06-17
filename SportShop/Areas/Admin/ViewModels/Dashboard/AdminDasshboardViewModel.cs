namespace SportShop.Areas.Admin.ViewModels.Dashboard
{
    public class AdminDashboardViewModel
    {
        public int TotalNewOrders { get; set; }  // Đơn hàng mới (Chờ duyệt)
        public decimal TotalRevenue { get; set; } // Tổng doanh thu (Đơn đã thanh toán)
        public int TotalProducts { get; set; }    // Tổng số sản phẩm
        public int TotalCustomers { get; set; }   // Tổng số khách hàng
    }
}