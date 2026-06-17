using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SportShop.Entities.Shop;
using SportShop.Models;
using System;
using System.Threading.Tasks;

namespace SportShop.Data.Repositories
{
    // Kế thừa thêm IDisposable để dọn dẹp DbContext khi UoW kết thúc
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Category> Category { get; }
        IRepository<Brand> Brand { get; }
        IRepository<Sport> Sport { get; }
        IProductRepository Product { get; }
        IRepository<ProductVariant> ProductVariant { get; }
        IRepository<ProductImage> ProductImage { get; }
        IRepository<User> User { get; }
        IRepository<UserAddress> UserAddress { get; }
        IRepository<Order> Order { get; }
        IRepository<OrderItem> OrderItem { get; }

        Task SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IRepository<Category> Category { get; private set; }
        public IRepository<Brand> Brand { get; private set; }
        public IRepository<Sport> Sport { get; private set; }
        public IProductRepository Product { get; private set; }
        public IRepository<ProductVariant> ProductVariant { get; private set; }
        public IRepository<ProductImage> ProductImage { get; private set; }
        public IRepository<User> User { get; private set; }
        public IRepository<UserAddress> UserAddress { get; private set; }
        public IRepository<Order> Order { get; private set; }
        public IRepository<OrderItem> OrderItem { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            // Tự động tạo một instance của DbContext dành riêng cho UnitOfWork này
            _context = context;

            // Truyền context vừa tạo cho tất cả các Repositories để chúng dùng chung 1 transaction
            Category = new Repository<Category>(_context);
            Brand = new Repository<Brand>(_context);
            Sport = new Repository<Sport>(_context);
            Product = new ProductRepository(_context);
            ProductVariant = new Repository<ProductVariant>(_context);
            ProductImage = new Repository<ProductImage>(_context);
            User = new Repository<User>(_context);
            UserAddress = new Repository<UserAddress>(_context);
            Order = new Repository<Order>(_context);
            OrderItem = new Repository<OrderItem>(_context);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}