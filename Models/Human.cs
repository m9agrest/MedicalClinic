using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MedicalClinic.Models
{
	public class Human
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		[Required]
		[StringLength(100)]
		public string Surname { get; set; }

		[StringLength(100)]
		public string Patronymic { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[StringLength(100)]
		public string Password { get; set; }

		/*
         * 0 - человек
         * 1 - доктор
         * 2 - администратор
         */
		[DefaultValue(0)]
		public int Type { get; set; }

		[StringLength(500)]
		public string? Description { get; set; }

		public int? Office { get; set; }

		[DefaultValue(true)]
		public bool IsActive { get; set; } = true;

		public virtual ICollection<HumanDoctorType>? HumanDoctorTypes { get; set; } = new List<HumanDoctorType>();
	}
}
