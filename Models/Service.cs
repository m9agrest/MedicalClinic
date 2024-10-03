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
        public string Description { get; set; }

        [Required]
        public string Price { get; set; }

        [Required]
        public virtual List<DoctorType> Executor { get; set; } // Взаимосвязь с DoctorType
    }
}
