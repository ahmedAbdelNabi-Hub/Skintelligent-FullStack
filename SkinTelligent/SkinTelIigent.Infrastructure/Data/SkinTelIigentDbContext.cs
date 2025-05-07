using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Entities.Appointment;
using SkinTelIigent.Core.Models;
using SkinTelIigent.Infrastructure.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Infrastructure.Data
{
    public class SkinTelIigentDbContext : IdentityDbContext
    {
        public DbSet<Doctor> Doctors{ get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Clinic> Clinic { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<DoctorPatient> DoctorPatient { get; set; }

        public DbSet<ClinicDoctor> ClinicDoctor { get; set; }
        public DbSet<Review> Reviews  { get; set; }
        public DbSet<Appointment> Appointments { get; set; }




        public SkinTelIigentDbContext(DbContextOptions<SkinTelIigentDbContext> options):base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        
       
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
           
            base.OnModelCreating(modelBuilder);

        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveColumnType("DateTime");
            configurationBuilder.Properties<decimal>().HaveColumnType("decimal(8,2)");
        }
    }
}
