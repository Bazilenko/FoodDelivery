using Microsoft.EntityFrameworkCore;
using Orders.Dal.Entities;
using Orders.Dal.Configurations;

namespace Orders.Dal.Context
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDish> OrderDishes => Set<OrderDish>();
        public DbSet<OrderDishOption> OrderDishOptions => Set<OrderDishOption>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<OrderStatusHistory> OrderStatusHistory => Set<OrderStatusHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDishConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDishOptionConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusHistoryConfiguration());
        }
    }
}