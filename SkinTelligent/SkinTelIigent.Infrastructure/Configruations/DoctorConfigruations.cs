using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkinTelIigent.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Infrastructure.Configruations
{
    public class DoctorConfigruations : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d=>d.IsApproved).HasDefaultValue(false);

            builder.HasOne(d => d.User)
                   .WithOne(u => u.Doctor)
                   .HasForeignKey<Doctor>(d => d.UserId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
