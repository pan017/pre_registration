using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using pre_registration.Models.DataBaseModel;
using System;
using System.Configuration;

namespace pre_registration.Models
{
    //public class ApplicationRole : IdentityRole<int>
    //{
    //    public ApplicationRole() : base()
    //    { }
    //    public ApplicationRole(string roleName) : base(roleName)
    //    {
    //    }
    //}
    public class ApplicationContext:DbContext
    {


        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            :base (options)
        {

        }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<DeniedCupon> DeniedCupons { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<ApplicationRole> Roles { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<CuponDate> CuponDates { get; set; }
        public DbSet<Denied> Denied { get; set; }
        public DbSet<UserData> UsersData { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ResetPassword> ResetPasswords { get; set; }
        public DbSet<SentNotification> SentNotifications { get; set; }
    }
}
