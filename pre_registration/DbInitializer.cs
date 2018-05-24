using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using pre_registration.Models;
using pre_registration.Models.DataBaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration
{
    public class DbInitializer : IDbInitializer
    {
        private readonly IServiceProvider _serviceProvider;

        public DbInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //This example just creates an Administrator role and one Admin users
        public async void Initialize()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                //create database schema if none exists
                var context = serviceScope.ServiceProvider.GetService<ApplicationContext>();
                context.Database.EnsureCreated();

                //If there is already an Administrator role, abort
                var _roleManager = serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
                if (!(await _roleManager.RoleExistsAsync("Administrator")))
                {
                    //Create the Administartor Role
                  //  await _roleManager.CreateAsync(new ApplicationRole("Administrator"));
                }
                //Create the default Admin account and apply the Administrator role
                var _userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                string user = "xxx@yyy.com";
                string password = "AbC!12345";
                var success = await _userManager.CreateAsync(new ApplicationUser { }, password);
                if (success.Succeeded)
                {
                    await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(user), "Administrator");
                }
            }
        }
    }
}
