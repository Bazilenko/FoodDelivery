using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Tracing;
using Catalog.Dal.Configurations;
using Catalog.Dal.Entities;
using Microsoft.EntityFrameworkCore;


namespace Catalog.Dal.Context
{
    public class MyDbContext : DbContext
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Restaurant> Restaurants => Set<Restaurant>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Dish> Dishes => Set<Dish>();
        public DbSet<DishOption> DishesOptions => Set<DishOption>();
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<Cuisine> Cuisines => Set<Cuisine>();
        public DbSet<WorkingHour> WorkingHours => Set<WorkingHour>();
        public DbSet<RestaurantCuisine> RestaurantCuisines => Set<RestaurantCuisine>();
        public DbSet<ModifierGroup> ModifierGroups => Set<ModifierGroup>();


        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB; Database = CatalogDB; Trusted_Connection = True; ");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new RestaurantConfiguration());
            builder.ApplyConfiguration(new AddressConfiguration());
            builder.ApplyConfiguration(new DishConfiguration());
            builder.ApplyConfiguration(new DishOptionConfiguration());
            builder.ApplyConfiguration(new ContactConfiguration());
            builder.ApplyConfiguration(new CuisineConfiguration());
            builder.ApplyConfiguration(new ModifierGroupConfiguration());
            builder.ApplyConfiguration(new RestaurantCuisineConfiguration());
            builder.ApplyConfiguration(new WorkingHoursConfiguration());

        }
    }
}
