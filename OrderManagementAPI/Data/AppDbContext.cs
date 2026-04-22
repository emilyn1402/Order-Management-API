using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Data
{
    //Manage database context
    public class AppDbContext : DbContext
    {
        //Constructor receives DB options from Program.cs
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
        }

        //These become tables
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderItemModel> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure decimal precision for money values
            modelBuilder.Entity<ProductModel>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderModel>()
                .Property(p => p.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItemModel>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
        }
    }
}
