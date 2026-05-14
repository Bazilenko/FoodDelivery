using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Dal.Configurations
{
    public class DishConfiguration : IEntityTypeConfiguration<Dish>
    {
        public void Configure(EntityTypeBuilder<Dish> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(d => d.Price)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
                
            builder.Property(d => d.Weight)
                .HasColumnType("decimal(8,2)");

            builder.Property(d => d.Unit)
                .HasMaxLength(20);

            builder.Property(d => d.Calories)
                .HasColumnType("decimal(8,2)");

            builder.Property(d => d.ImageUrl)
                .HasMaxLength(500);
            
            builder.HasQueryFilter(d => !d.IsDeleted);

            builder.HasOne(d => d.Category)
                .WithMany(c => c.Dishes)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Restaurant)
                .WithMany(r => r.Dishes)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.ModifierGroups)
                .WithOne(m => m.Dish)
                .HasForeignKey(m => m.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            
                
                

        }
    }
}
