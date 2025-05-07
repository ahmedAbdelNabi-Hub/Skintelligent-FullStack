using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkinTelIigent.Core.Entities;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace SkinTelIigent.Infrastructure.Configruations
{
    public class DoctorPatientConfigurations : IEntityTypeConfiguration<DoctorPatient>
    {
        public void Configure(EntityTypeBuilder<DoctorPatient> builder)
        {
            builder.HasKey(cd => new { cd.PatientId, cd.DoctorId });
            builder.Ignore(dp => dp.Id);

            builder.HasOne(cd => cd.Patient)
             .WithMany(c => c.DoctorPatients)
             .HasForeignKey(cd => cd.PatientId)
             .OnDelete(DeleteBehavior.NoAction);  

            builder.HasOne(cd => cd.Doctor)
                .WithMany(d => d.DoctorPatients)
                .HasForeignKey(cd => cd.DoctorId)
                .OnDelete(DeleteBehavior.Cascade); 

        }
    }
}
