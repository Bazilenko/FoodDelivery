using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Dal.Configurations
{
    public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Rating)
                .IsRequired()
                .HasColumnType("decimal(3,2)");

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(r => r.ImageUrl)
                .HasMaxLength(500);

            builder.Property(r => r.DeliveryRadiusKm)
                .HasColumnType("decimal(6,2)");
            
            builder.HasQueryFilter(r => !r.IsDeleted);

            builder.HasMany(r => r.Dishes)
                .WithOne(d => d.Restaurant)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Contacts)
                .WithOne(c => c.Restaurant)
                .HasForeignKey(c => c.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.WorkingHours)
                .WithOne(w => w.Restaurant)
                .HasForeignKey(w => w.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Addresses)
                .WithOne(a => a.Restaurant)
                .HasForeignKey(a => a.RestaurantId) 
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.RestaurantCuisines)
                .WithOne(rc => rc.Restaurant)
                .HasForeignKey(rc => rc.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Categories)
                .WithOne(c => c.Restaurant)
                .HasForeignKey(c => c.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
                   
        }
    }
}
