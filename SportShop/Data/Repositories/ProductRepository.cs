using Microsoft.EntityFrameworkCore;
using SportShop.Entities.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportShop.Data.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count, string includeProperties = null);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, int count, string includeProperties = null);
    }

    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count, string includeProperties = null)
        {
            IQueryable<Product> query = _context.Products;

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            // ==========================================
            // LOGIC SẢN PHẨM NỔI BẬT
            // ==========================================
            // Lấy các sản phẩm mới nhất và trộn ngẫu nhiên 
            // để mỗi lần khách vào trang chủ sẽ thấy slider chạy các sản phẩm khác nhau.
            // Sắp xếp theo ID giảm dần để lấy hàng mới, sau đó xáo trộn (Random)

            var latestProducts = await query
                .OrderByDescending(p => p.Id)
                .Take(count * 2) // Lấy dư ra gấp đôi số lượng cần thiết
                .ToListAsync();

            // Xáo trộn ngẫu nhiên danh sách và lấy đúng số lượng (Ví dụ: 8 hoặc 12 cái để Slider chạy cho đẹp)
            var random = new Random();
            return latestProducts.OrderBy(x => random.Next()).Take(count).ToList();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, int count, string includeProperties = null)
        {
            IQueryable<Product> query = _context.Products.Where(p => p.CategoryId == categoryId);

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.Take(count).ToListAsync();
        }
    }
}