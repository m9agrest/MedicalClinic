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
        [Required]
        public int Type { get; set; }
    }
}
