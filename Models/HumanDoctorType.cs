using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalClinic.Models
{
	public class HumanDoctorType
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int HumanId { get; set; }

		[ForeignKey("HumanId")]
		public virtual Human Human { get; set; }

		[Required]
		public int DoctorTypeId { get; set; }

		[ForeignKey("DoctorTypeId")]
		public virtual DoctorType DoctorType { get; set; }
	}
}
