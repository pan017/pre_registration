using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using pre_registration.Models;
using pre_registration.Models.DataBaseModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pre_registration.Services
{
    public class DbInitializer
    {
        public static async void Initialize(ApplicationContext context)
        {
           if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new ApplicationRole { Name = "user" },
                    new ApplicationRole { Name = "superuser" },
                    new ApplicationRole { Name = "admin" });
                context.SaveChanges();

            }
           if (!context.Users.Any())
            {
                UserService.CreateUser(new Models.ViewModels.AddUserViewModel
                {
                    FirstName = "Илья",
                    LastName = "Паныш",
                    SecondName = "Сергеевич",
                    Login = "pan017@yandex.by",
                    Password = "a2l6e3x2IS",
                    PasswordConfirm = "a2l6e3x2IS",
                    Phone = "+375291849927",
                    RoleId = context.Roles.FirstOrDefault(x => x.Name == "admin").Id                    
                }, context);
            }
        }
    }
}
