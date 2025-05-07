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
    public class PatientConfigruations : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(d => d.User)
                   .WithOne(u => u.Patient)
                   .HasForeignKey<Patient>(d => d.UserId)
                   .OnDelete(DeleteBehavior.Cascade);  

              builder.HasMany(a => a.Appointments)
    .WithOne(p => p.Patient)
    .HasForeignKey(a => a.PatientId)
    .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
