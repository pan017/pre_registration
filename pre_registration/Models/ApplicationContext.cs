using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace pre_registration.Models
{
    public class ApplicationContext:IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            :base (options)
        {           
        }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<CuponDate> CuponDates { get; set; }
        public DbSet<Denied> Denied { get; set; }
     //   public DbSet<AdminUser> AdminUsers { get; set; }
    }
}
