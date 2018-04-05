using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pre_registration.Models
{
    public class Area
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название района")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Электронная почта для уведомлений")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }


    }
}
