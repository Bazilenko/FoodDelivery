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
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {

        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Type)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Value)
                .IsRequired()
                .HasMaxLength(300);

            builder.HasOne(c => c.Restaurant)
                .WithMany(r => r.Contacts)
                .HasForeignKey(c => c.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
