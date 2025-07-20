using EpicPanels_Group5_FinalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace EpicPanels_Group5_FinalProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<CustomerReview> CustomerReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // OrderDetail Configurations
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            // CustomerReview Configurations
            modelBuilder.Entity<CustomerReview>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerReview>()
                .HasOne(r => r.Product)
                .WithMany(p => p.CustomerReviews)
                .HasForeignKey(r => r.ProductID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
