using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        [Display(Name = "Логин")]
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

        [Required]
        [Display(Name = "Район")]
        public int AreaID { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public string RoleId { get; set; }
    }
}
