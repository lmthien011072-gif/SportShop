using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportShop.Areas.Admin.ViewModels;
using SportShop.Areas.Admin.ViewModels.Dashboard;
using SportShop.Data.Repositories;
using SportShop.Entities;
using SportShop.Entities.Shop;
using System.Linq;
using System.Threading.Tasks;

namespace SportShop.Services
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardViewModel> GetDashboardStatisticsAsync();
    }

    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public AdminDashboardService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<AdminDashboardViewModel> GetDashboardStatisticsAsync()
        {
            // 1. Thống kê Đơn hàng
            var allOrders = await _unitOfWork.Order.GetAllAsync();

            // Đếm số đơn hàng đang ở trạng thái "Chờ duyệt" (Pending)
            int newOrdersCount = allOrders.Count(o => o.Status == OrderStatus.Pending);

            // Tính tổng doanh thu: Chỉ cộng tiền các đơn đã thanh toán thành công
            decimal totalRevenue = allOrders
                .Where(o => o.PaymentStatus == PaymentStatus.Paid)
                .Sum(o => o.TotalAmount);

            // 2. Thống kê Sản phẩm
            var allProducts = await _unitOfWork.Product.GetAllAsync();
            int totalProducts = allProducts.Count();

            // 3. Thống kê Khách hàng (Sử dụng Entity Framework để đếm trực tiếp)
            // Lưu ý: Tạm đếm tổng số User. Nếu hệ thống lớn có thể thêm query lọc Role
            int totalCustomers = await _userManager.Users.CountAsync();

            return new AdminDashboardViewModel
            {
                TotalNewOrders = newOrdersCount,
                TotalRevenue = totalRevenue,
                TotalProducts = totalProducts,
                TotalCustomers = totalCustomers
            };
        }
    }
}