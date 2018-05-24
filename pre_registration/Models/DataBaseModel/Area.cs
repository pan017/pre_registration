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
        public string NotificationEmail { get; set; }
        [Required]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }
        [Required]
        [Display(Name = "Адрес")]
        public string Adres { get; set; }
        [Required]
        [Display(Name = "Время работы")]
        public string WorkTime { get; set; }
        [Required]
        [Display(Name = "Сайт")]
        public string website { get; set; }
        [Required]
        [Display(Name = "Почта")]
        public string email { get; set; }


    }
}
