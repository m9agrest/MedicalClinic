using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalClinic.Models
{
	public class ServiceList
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public DateTime DateTime { get; set; }

		[Required]
		public int ClientId { get; set; }

		[ForeignKey("ClientId")]
		public Human Client { get; set; }

		[Required]
		public int DoctorId { get; set; }

		[ForeignKey("DoctorId")]
		public Human Doctor { get; set; }

		[Required]
		public int ServiceId { get; set; }

		[ForeignKey("ServiceId")]
		public Service Service { get; set; }

		[Required]
		public int Price { get; set; }

		[StringLength(500)]
		public string? Description { get; set; }
	}
}
