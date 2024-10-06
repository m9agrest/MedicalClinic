using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalClinic.Models
{
	public class Service
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(200)]
		public string Name { get; set; }

		[StringLength(500)]
		public string? Description { get; set; }

		[Required]
		public int Price { get; set; }

		[DefaultValue(true)]
		public bool IsActive { get; set; }

		[Required]
		public virtual ICollection<ServiceDoctorType> ServiceDoctorTypes { get; set; } = new List<ServiceDoctorType>();
	}
}
