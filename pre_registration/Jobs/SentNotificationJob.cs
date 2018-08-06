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
    public class SentNotificationJob: IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            string connectionString = context.JobDetail.JobDataMap.GetString("connectionString");
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
                    item.Order.Client = db.Clients.FirstOrDefault(x => x.id == item.Order.ClientId);
                    item.Order.Client.UserData = db.UsersData.FirstOrDefault(x => x.id == item.Order.Client.UserDataID);
                    item.Order.CuponDate = db.CuponDates.FirstOrDefault(x => x.id == item.Order.CuponDateId);
                    item.Order.CuponDate.Area = db.Areas.FirstOrDefault(x => x.Id == item.Order.CuponDate.AreaId);
                }
                 foreach (var item in sentNotificationsList)
                {
                    System.TimeSpan diff1 = item.Order.CuponDate.date.AddDays(1) - DateTime.Now;
                    if (diff1.Days <= 1)
                    {
                        DeniedCupon deniedCupon = new DeniedCupon();
                        deniedCupon = db.DeniedCupons.FirstOrDefault(x => x.OrderId == item.OrderId);
                        Services.EmailService.SendMail(new Services.NotificationEmail { DisplayngName = "Предварительная запись в службу Одно окно", Login = "notification.prereg@mgaon.by", Password = "4rfvBGT5" },
                            item.Order.Client.UserData.EmailAdress, "Напоминание о записи на прием в службу «одно окно»", GetMessageBody(item.Order, deniedCupon));
                        item.isSent = true;
                        db.SaveChanges();
                    }
                }
            }
            return Task.CompletedTask;
        }
        private string GetMessageBody(Order order, DeniedCupon deniedCupon)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            return String.Format(@"
                <html>
                <body>
                <br>
                <br>Здравствуйте, {0} {1} {2}, <br>Напоминаем вам, что Вы забронировали время подачи документов в службе «одно окно» {3} на {4} в {5}.
                <p><b>Если Вы не можете</b> прийти в забронированное время, то, пожалуйста, <b>аннулируйте</b> бронирование здесь {6} </p>
                <br>Это письмо было сгенерировано автоматически. Отвечать на него не нужно.
                <p>Служба «одно окно» {7}.<br>
                {8}<br>
                {9}</p>
                </body>
                </html>",
              order.Client.UserData.LastName,
              order.Client.UserData.FirstName,
              order.Client.UserData.SecondName,

              Helpers.getAreaNameDeclination(order.CuponDate.Area.Name),
              order.CuponDate.date.ToShortDateString(),
              order.CuponDate.date.ToShortTimeString(),
              String.Format(@"<a href='http://prereg.mgaon.by/Cupon/DeniedCupon?key={0}'>Отменить запись</a>", deniedCupon.DeniedKey),
              Helpers.getAreaNameDeclination(order.CuponDate.Area.Name),
              order.CuponDate.Area.Adres,
              order.CuponDate.Area.Phone);
        }
    }
}
