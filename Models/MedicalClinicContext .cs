using Microsoft.EntityFrameworkCore;

namespace MedicalClinic.Models
{
	public class MedicalClinicContext : DbContext
	{
		public MedicalClinicContext(DbContextOptions<MedicalClinicContext> options)
			: base(options) // передаем параметры в базовый конструктор
		{
		}

		public DbSet<Human> Humans { get; set; }
		public DbSet<DoctorType> DoctorTypes { get; set; }
		public DbSet<Service> Services { get; set; }
		public DbSet<ServiceList> ServiceLists { get; set; }
		public DbSet<HumanDoctorType> HumanDoctorTypes { get; set; }
		public DbSet<ServiceDoctorType> ServiceDoctorTypes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<ServiceList>()
				.HasOne(s => s.Client)
				.WithMany()
				.HasForeignKey(s => s.ClientId)
				.OnDelete(DeleteBehavior.Restrict); // Измените на Restrict

			modelBuilder.Entity<ServiceList>()
				.HasOne(s => s.Doctor)
				.WithMany()
				.HasForeignKey(s => s.DoctorId)
				.OnDelete(DeleteBehavior.Restrict); // Измените на Restrict

			modelBuilder.Entity<ServiceList>()
				.HasOne(s => s.Service)
				.WithMany()
				.HasForeignKey(s => s.ServiceId)
				.OnDelete(DeleteBehavior.Cascade);
		}

	}
}
