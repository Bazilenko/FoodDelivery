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
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);


            builder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.BuildingNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(a => a.PostalCode)
                .IsRequired(false)
                .HasMaxLength(20);

            builder.Property(a => a.Latitude)
                .IsRequired()
                .HasColumnType("decimal(9,6)");
            
            builder.Property(a => a.Longitude)
                .IsRequired()
                .HasColumnType("decimal(9,6)");

            builder.HasQueryFilter(a => !a.IsDeleted);

            builder.HasOne(a => a.Restaurant)
                .WithMany(r => r.Addresses)
                .HasForeignKey(a => a.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
