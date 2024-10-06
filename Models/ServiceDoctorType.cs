using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalClinic.Models
{
	public class ServiceDoctorType
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int ServiceId { get; set; }

		[ForeignKey("ServiceId")]
		public virtual Service Service { get; set; }

		[Required]
		public int DoctorTypeId { get; set; }

		[ForeignKey("DoctorTypeId")]
		public virtual DoctorType DoctorType { get; set; }
	}
}
