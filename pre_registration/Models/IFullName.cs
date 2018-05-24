using System.ComponentModel.DataAnnotations;


namespace pre_registration.Models
{
    interface IFullName
    {
        [Required]
        [Display(Name = "Фамилия")]
        string LastName { get; set; }

        [Required]
        [Display(Name = "Имя")]
        string FirstName { get; set; }

        [Required]
        [Display(Name = "Отчетсво")]
        string SecondName { get; set; }
    }
}
