using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.ViewModels
{
    public class SettingsViewModel
    {
        public int id { get; set; }

        public string Login { get; set; }
        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Отчетсво")]
        public string SecondName { get; set; }


        [Required]
        [Display(Name = "Номер телефона")]
        public string Phone { get; set; }
        [Display(Name = "Присылать уведомления с информацией о заказе и отмене талона")]
        public bool SendEmail { get; set; }
        [Display(Name = "Присылать напоминание накануне даты приёма по талону")]
        public bool SendReminder { get; set; }


    }
}
