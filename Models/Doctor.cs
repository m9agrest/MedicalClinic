using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MedicalClinic.Models
{
    public class Doctor
    {
        [Key]
        public int HumanId { get; set; } // Внешний ключ на Human

        [ForeignKey("HumanId")]
        public virtual Human Human { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } // Обязательное поле с максимальной длиной

        [Required]
        public int Office { get; set; } // Обязательное поле

        [Required]
        public virtual List<DoctorType> Type { get; set; } // Взаимосвязь с DoctorType
    }
}
