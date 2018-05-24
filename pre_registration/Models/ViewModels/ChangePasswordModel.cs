using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.ViewModels
{
    public class ChangePasswordModel
    {
        public int id { get; set; }
        [Display(Name = "Старый пароль")]
        public string oldPassword { get; set; }
        [Display(Name = "Новый пароль")]
        public string newPassword { get; set; }
        [Compare("newPassword", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string confirmPassword { get; set; }
    }
}
