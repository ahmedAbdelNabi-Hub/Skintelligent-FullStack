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
    public class NotificationConfigruations : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(n => n.Message)
                   .IsRequired();

            builder.Property(n => n.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()")
                   .IsRequired();

            builder.Property(n => n.IsRead)
                   .HasDefaultValue(false);

            builder.HasOne(n => n.User)
                   .WithMany(u => u.Notifications)
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
