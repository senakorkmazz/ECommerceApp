using ECommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; } // Ürün ekleme işlemi için MSSQL tarafında da Product tablosu

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // MSSQL'deki Product tablosundan MongoId alanını çıkarabiliriz
            // Veya bu alanı MSSQL'de de tutabiliriz, ancak bu örnekte görmezden gelelim.
            modelBuilder.Entity<Product>()
                .Ignore(p => p.MongoId); // EF Core'un bu alanı maplememesini sağlar

            // Kullanıcı adının unique olmasını sağlayabiliriz (opsiyonel)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}