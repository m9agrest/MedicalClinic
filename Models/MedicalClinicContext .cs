using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MedicalClinic.Models
{
    public class MedicalClinicContext : DbContext
    {
        public MedicalClinicContext(DbContextOptions<MedicalClinicContext> options)
            : base(options)
        {
        }

        // Таблицы
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorType> DoctorTypes { get; set; }
        public DbSet<Human> Humans { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceList> ServiceLists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Определение связей и конфигураций

            // Связь Doctor с Human
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Human)
                .WithMany() // В случае, если у Human несколько докторов, здесь можно использовать .WithMany(h => h.Doctors)
                .HasForeignKey(d => d.HumanId);

            // Связь Service с DoctorType (многие ко многим)
            modelBuilder.Entity<Service>()
                .HasMany(s => s.Executor)
                .WithMany();

            // Связь ServiceList с Doctor
            modelBuilder.Entity<ServiceList>()
                .HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(sl => sl.DoctorId);

            // Связь ServiceList с Service
            modelBuilder.Entity<ServiceList>()
                .HasOne<Service>()
                .WithMany()
                .HasForeignKey(sl => sl.ServiceId);

            // Связь ServiceList с Human (ClientId)
            modelBuilder.Entity<ServiceList>()
                .HasOne<Human>()
                .WithMany()
                .HasForeignKey(sl => sl.ClientId);
        }
    }
}
