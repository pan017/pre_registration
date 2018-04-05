using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;

namespace pre_registration.Models
{
    public class ApplicationRole : IdentityRole<int>
    {

    }
    public class ApplicationContext:IdentityDbContext<User, ApplicationRole, int>
    {


        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            :base (options)
        {

        }

        public DbSet<Area> Areas { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<CuponDate> CuponDates { get; set; }
        public DbSet<Denied> Denied { get; set; }
        public DbSet<UserData> UsersData { get; set; }

    }
}
