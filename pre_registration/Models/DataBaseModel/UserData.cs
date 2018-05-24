using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models
{
    public class UserData
    {
        public int id { get; set; }
        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Отчество")]
        public string SecondName { get; set; }
        [Required]
        [Display(Name = "Номер телефона")]
        public string Phone { get; set; }
        [Required]
        [Display(Name = "Адрес электронной почты")]
        public string EmailAdress { get; set; }
    }
}
