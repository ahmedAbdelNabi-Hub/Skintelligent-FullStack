using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkinTelIigent.Core.Entities.Appointment;
using System;
using System.Reflection.Emit;

namespace SkinTelIigent.Infrastructure.Configurations
{
    public class AppointmentConfigurations : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
        
            builder.ToTable("Appointments");

            builder.HasKey(a => a.Id); 

            builder.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)  
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);  

            builder.HasOne(a => a.Clinic)
                .WithMany(c => c.Appointments) 
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Property(a => a.StartTime)
                .IsRequired();

            builder.Property(a => a.EndTime)
                .IsRequired();

            builder.Property(a => a.IsCanceled)
                .HasDefaultValue(false);

            builder.Property(a => a.IsRepeating)
                .HasDefaultValue(false);

            builder.Property(a => a.RepeatDay)
                .IsRequired(false);

            builder.Property(a => a.RepeatUntil)
                .IsRequired(false);

            builder.HasIndex(a => new { a.DoctorId, a.StartTime })
            .IsUnique(false);

            builder
            .Property(e => e.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        }
    }
}
