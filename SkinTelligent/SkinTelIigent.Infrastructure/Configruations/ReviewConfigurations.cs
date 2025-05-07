using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkinTelIigent.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Infrastructure.Configruations
{
    public class ReviewConfigurations : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {

            builder.HasOne(r => r.Clinic)
          .WithMany()
          .HasForeignKey(r => r.ClinicId)
          .OnDelete(DeleteBehavior.Cascade); 

            builder.Property(r => r.Comment)
                .IsRequired()
                .HasMaxLength(300);  

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(n => n.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()")
              .IsRequired();
        }
    }
}
