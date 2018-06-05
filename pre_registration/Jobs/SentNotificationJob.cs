using Microsoft.EntityFrameworkCore;
using pre_registration.Models;
using pre_registration.Models.DataBaseModel;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Jobs
{
    public class SentNotificationJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            string connectionString = "Server=.; Database=PreRegistrationDB;Integrated Security=true";//"Server = tcp:172.16.209.203, 1433; Database = PreRegistrationDB.mdf; User Id = sa; Password = QWEasdZXC123; Integrated Security = true";
                                                                                                      //string connectionString = ConfigurationManager.ConnectionStrings["DataBaseConnection"].ConnectionString;
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            var options = optionsBuilder
                .UseSqlServer(connectionString)
                .Options;
            using (ApplicationContext db = new ApplicationContext(options))
            {
                List<SentNotification> sentNotificationsList = db.SentNotifications.Where(x => x.isSent == false).ToList();
                foreach (var item in sentNotificationsList)
                {
                    item.Order = db.Orders.FirstOrDefault(x => x.id == item.OrderId);
                }
                foreach (var item in sentNotificationsList)
                {
                    if (item.Order.OrderDate.AddDays(1) > DateTime.Now)
                    {
                        //TODO SendMail
                        item.isSent = true;
                        db.SaveChanges();
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
