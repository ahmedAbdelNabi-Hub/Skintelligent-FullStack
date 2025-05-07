using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkinTelIigent.Core.Entities;

namespace SkinTelIigent.Infrastructure.Configruations
{
    public class ClinicDoctorConfigurations : IEntityTypeConfiguration<ClinicDoctor>
    {
        public void Configure(EntityTypeBuilder<ClinicDoctor> builder)
        {
            builder.HasKey(cd => new { cd.ClinicId, cd.DoctorId });

            builder.Ignore(cd => cd.Id);

            builder.HasOne(cd => cd.Clinic)
                .WithMany(c => c.ClinicDoctors)
                .HasForeignKey(cd => cd.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);  // Deleting a Clinic removes ClinicDoctor relationships

            builder.HasOne(cd => cd.Doctor)
                .WithMany(d => d.ClinicDoctors)
                .HasForeignKey(cd => cd.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);  // Deleting a Doctor does not affect ClinicDoctor relationships
        }
    }
}
