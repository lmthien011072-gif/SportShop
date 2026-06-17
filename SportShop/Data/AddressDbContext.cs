using Microsoft.EntityFrameworkCore;
using SportShop.Entities.Location;

namespace SportShop.Data
{
    public class AddressDbContext : DbContext
    {
        public AddressDbContext(DbContextOptions<AddressDbContext> options) : base(options)
        {
        }

        public DbSet<Province> Provinces { get; set; }
        public DbSet<Commune> Communes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình quan hệ Tỉnh - Xã
            modelBuilder.Entity<Commune>()
                .HasOne(c => c.Province)
                .WithMany(p => p.Communes)
                .HasForeignKey(c => c.ProvinceCode)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
