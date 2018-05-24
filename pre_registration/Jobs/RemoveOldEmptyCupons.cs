using Microsoft.EntityFrameworkCore;
using pre_registration.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using pre_registration.Models.DataBaseModel;
namespace pre_registration.Jobs
{
    public class RemoveOldEmptyCupons : IJob
    {      

        public Task Execute(IJobExecutionContext context)
        {
            string connectionString = "Server = tcp:172.16.209.203, 1433; Database = PreRegistrationDB.mdf; User Id = sa; Password = QWEasdZXC123; Integrated Security = true";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            var options = optionsBuilder
                .UseSqlServer(connectionString)
                .Options;
            using (ApplicationContext db = new ApplicationContext(options))
            {
                List<Order> orders = db.Orders.ToList();
                List<CuponDate> allCuponList = db.CuponDates.Where(x => x.date < DateTime.Now).ToList();
                List<CuponDate> busyCupons = new List<CuponDate>();

                foreach (var cupon in allCuponList)
                {
                    foreach (var order in orders)
                    {
                        if (cupon.id == order.CuponDateId)
                            busyCupons.Add(cupon);
                    }
                }
                List<CuponDate> oldItems = allCuponList.Except(busyCupons).ToList();                
                foreach (var item in oldItems)
                {
                    db.CuponDates.Remove(item);
                    db.SaveChanges();
                }
                return Task.CompletedTask;
            }
        }
    }
}
