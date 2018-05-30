using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "Адрес электронной почты или номер телефона")]
        public string Query { get; set; }
        public DataBaseModel.ResetPassword ResetPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Повторите новый пароль")]
        public string ConfirmNewPassword { get; set; }
    }
}
