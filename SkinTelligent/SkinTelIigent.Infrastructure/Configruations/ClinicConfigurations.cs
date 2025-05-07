using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace SkinTelIigent.Infrastructure.Configruations
{
    public class ClinicConfigurations : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.IsApproved).HasDefaultValue(false);

            builder.HasOne(d => d.User).WithOne(d => d.Clinic).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
