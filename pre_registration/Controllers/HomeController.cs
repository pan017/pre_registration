using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pre_registration.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using pre_registration.Models.DataBaseModel;
using System.Security.Claims;

using pre_registration.Services;
using System.Configuration;
using Microsoft.Extensions.Options;

namespace pre_registration.Controllers
{
    public class HomeController : Controller
    {
        ApplicationContext db;
        private readonly IOptions<AppConfig> config;
        public HomeController(ApplicationContext context, IOptions<AppConfig> config)
        {

            db = context;
            this.config = config;           
        }

        public IActionResult returnToSelectArea()
        {
            HttpContext.Session.Remove("Area");
            HttpContext.Session.Remove("CuponId");
            HttpContext.Session.Remove("Date");
            return RedirectToAction("selectAreaForm");
        }
        public IActionResult returnToSelectDate()
        {
            HttpContext.Session.Remove("Date");
            HttpContext.Session.Remove("CuponId");
            return RedirectToAction("viewCalendar", "Cupon", new { areaId = HttpContext.Session.GetInt32("Area")});
        }
        public IActionResult returnToSelectTime()
        {
            HttpContext.Session.Remove("CuponId");
            //var stringDate = HttpContext.Session.GetString("Date"); 
            //int[] dateComponents = stringDate.Split('.').Select(n => Convert.ToInt32(n)).ToArray();
            //DateTime selectDay = new DateTime(dateComponents[2], dateComponents[1], dateComponents[0]);
            return RedirectToAction("viewTime", "Cupon", new { selectedDay = GetDateFromSession(), areaId = HttpContext.Session.GetInt32("Area") });
        }
        private DateTime GetDateFromSession()
        {
            try
            {
                var stringDate = HttpContext.Session.GetString("Date");
                int[] dateComponents = stringDate.Split('.').Select(n => Convert.ToInt32(n)).ToArray();
                return new DateTime(dateComponents[2], dateComponents[1], dateComponents[0]);

            }
            catch (Exception e)
            {
                return new DateTime();
            }
        }
       
        public Area getSessionArea()
        {
            if (String.IsNullOrEmpty(HttpContext.Session.GetInt32("Area").ToString()))
                return new Area();
            else
            {
                return db.Areas.FirstOrDefault(x => x.Id == HttpContext.Session.GetInt32("Area"));
            }
                
        }
        public string getSelectedDate()
        {

            if (String.IsNullOrEmpty(HttpContext.Session.GetInt32("Area").ToString()))
                return "";
            else
                return HttpContext.Session.GetString("Date");
        }
        public CuponDate GetSelectedCupon()
        {
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("CuponId")))
                return new CuponDate();
            else
                return db.CuponDates.FirstOrDefault(x => x.id == int.Parse(HttpContext.Session.GetString("CuponId")));
        }

        public IActionResult showAreaInfo(int areaId)
        {
            
            return PartialView(db.Areas.FirstOrDefault(x => x.Id == areaId));
        }
        public IActionResult GetMap(int areaId)
        {
            return PartialView(db.Areas.FirstOrDefault(x => x.Id == areaId));
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult selectTime(string cuponId)
        {

            return PartialView();
        }
        public IActionResult selectAreaForm()
        {
            List<Area> AreasList = db.Areas.ToList();
            //for (int i = 0; i < AreasList.Count; i++)
            //{
            //    AreasList[i].Name = AreasList[i].Name == "Мингорисполком" ? AreasList[i].Name : AreasList[i].Name + " район";
            //}
            ViewBag.SelectedArea = HttpContext.Session.GetInt32("Area");
            DateTime selectedDate = new DateTime();
            DateTime.TryParse(HttpContext.Session.GetString("Date"), out selectedDate);
            ViewBag.selectedDate = GetDateFromSession();
            SelectList areasSelectList = new SelectList(AreasList, "Id", "Name");
            ViewBag.AreasList = db.Areas.ToList(); //areasSelectList;
            
            return PartialView();
        }
        public IActionResult recordForm(string id)
        {
            if (User.Identity.IsAuthenticated)
            {

                int userId = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name).Id;
                var userDataId = db.Users.Where(x => x.Id == userId).First().UserDataID;
                var userData = db.UsersData.Where(x => x.id == userDataId).First();
                Client userClient = new Client();
                userClient.UserData = userData;
                


                Order order = new Order();
                order.Client = userClient;
                order.CuponDate = db.CuponDates.First(x => x.id == int.Parse(id));
                ViewBag.order = order;
            }
            
            HttpContext.Session.SetString("CuponId", id);
            return PartialView();
        }
        [HttpPost]
        public IActionResult recordForm(Order model)
        {
            string captchaResponse = HttpContext.Request.Form["g-Recaptcha-Response"];
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://www.google.com");

            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>
            ("secret", "6LcdnGIUAAAAAP_4UrRGJOTEaofjTPxbbfyu5Xtm"));
            values.Add(new KeyValuePair<string, string>
             ("response", captchaResponse));
            FormUrlEncodedContent content = new FormUrlEncodedContent(values);
            HttpResponseMessage response = client.PostAsync("/recaptcha/api/siteverify", content).Result;

            string verificationResponse = response.Content.
            ReadAsStringAsync().Result;
            var res = JsonConvert.DeserializeObject<ReCaptchaValidationResult>(verificationResponse);
            if (!res.Success)
            {
                ModelState.AddModelError("", "Ошибка! Вы не прошли проверку безопасности. Пожалуйста, повторите ещё раз.");
                return RedirectToAction("recordForm", "Home", model);
            }
            
            CuponDate cuponDate = db.CuponDates.First(x => x.id == int.Parse(HttpContext.Session.GetString("CuponId")));
            Order newOrder = new Order();

            ApplicationUser applicationUser = new ApplicationUser();// = db.Users.FirstOrDefault(x => x.Id == userId);
            
            newOrder.CuponDateId = db.CuponDates.First(x => x.id == int.Parse(HttpContext.Session.GetString("CuponId"))).id;
            newOrder.OrderDate = DateTime.Now;
            newOrder.Comment = model.Comment;
            if (User.Identity.IsAuthenticated)
            {
                int userId = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name).Id;
                Client userClient = db.Clients.Where(x => x.User.Id == userId).FirstOrDefault();
                applicationUser = db.Users.FirstOrDefault(x => x.Id == userId);
                applicationUser.UserSettings = db.UserSettings.FirstOrDefault(x => x.id == applicationUser.UserSettingsId);

                if (userClient == null)
                {
                    Client newClient = new Client();
                    newClient.UserId = userId;
                    newClient.UserDataID = db.Users.First(x => x.Id == userId).UserDataID;
                    db.Clients.Add(newClient);
                    db.SaveChanges();
                    userClient = newClient;
                   
                }
                newOrder.ClientId = userClient.id;
            }
            else
            {
                var userData = model.Client.UserData;
                db.UsersData.Add(userData);
                db.SaveChanges();
                pre_registration.Models.Client newClient = new pre_registration.Models.Client();
                newClient.UserDataID = userData.id;
                db.Clients.Add(newClient);
                db.SaveChanges();
                newOrder.ClientId = newClient.id;
            }
            newOrder.Client = db.Clients.FirstOrDefault(x => x.id == newOrder.ClientId);
            newOrder.Client.UserData = db.UsersData.FirstOrDefault(x => x.id == newOrder.Client.UserDataID);
            newOrder.CuponDate = db.CuponDates.FirstOrDefault(x => x.id == newOrder.CuponDateId);
            newOrder.CuponDate.Area = db.Areas.FirstOrDefault(x => x.Id == newOrder.CuponDate.AreaId);
            db.Orders.Add(newOrder);
            db.SaveChanges();
            DeniedCupon deniedCupon = new DeniedCupon(newOrder.id, Helpers.ConvertStringtoMD5(Guid.NewGuid().ToString()));
            db.DeniedCupons.Add(deniedCupon);
            db.SaveChanges();
            
            if (applicationUser.UserSettings != null)
            {
                if (applicationUser.UserSettings.SendEmail)
                {
                    EmailService.SendMail(config.Value.NotificationEmail, newOrder.Client.UserData.EmailAdress, "Запись в службу 'Одно окно'", getMessageBody(newOrder, deniedCupon));
                }
                if (applicationUser.UserSettings.SendReminder)
                {
                    SentNotification sentNotification = new SentNotification();
                    sentNotification.isSent = false;
                    sentNotification.OrderId = newOrder.id;
                    db.SentNotifications.Add(new SentNotification
                    {
                        isSent = false,
                        OrderId = newOrder.id
                    });
                    db.SaveChanges();
                }
            }
            else
            {
                EmailService.SendMail(config.Value.NotificationEmail, newOrder.Client.UserData.EmailAdress, "Запись в службу 'Одно окно'", getMessageBody(newOrder, deniedCupon));
            }        
            HttpContext.Session.Remove("Date");
            HttpContext.Session.Remove("Area");
            HttpContext.Session.Remove("CuponId");
            HttpContext.Session.Remove("continueWithOutRegistration");         
           
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private string getAreaNameDeclination(string areaName)
        {
            if (areaName == "Мингорисполком")
                return areaName;
            else
            {
                string result = areaName;//.Replace("кий", "ого");
                result = result.Replace("кий", "ого");
                result = result.Replace("ный", "ного");
                result = result.Replace("кой", "кого");
                result = result + " района";
                return result;
            }
        }
        public string getMessageBody(Order order, DeniedCupon deniedCupon)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            return String.Format(@"
                <html>
                <body>
                <br>
                <br>Здравствуйте {0} {1} {2}, <br> Вы забронировали время подачи документов в службе Одно окно {3} на {4} в {5}.
                Желательно распечатать это сообщение и принести его с собой на подачу документов.
                <p><b>Если Вы не можете</b> прийти в забронированное время, то, пожалуйста, <b>аннулируйте</b> бронирование здесь {6} </p>
                <br>Это письмо было сгенерировано автоматически. Пожалуйста, не отвечайте на него.
                <p>Служба Одно окно {7}.<br>
                {8}<br>
                {9}</p>
                </body>
                </html>",
              order.Client.UserData.LastName,
              order.Client.UserData.FirstName,
              order.Client.UserData.SecondName,

              getAreaNameDeclination(order.CuponDate.Area.Name),
              order.CuponDate.date.ToShortDateString(),
              order.CuponDate.date.ToShortTimeString(),
              String.Format(@"<a href='http://{0}/Cupon/DeniedCupon?key={1}'>Отменить запись</a>", Request.Host.Value, deniedCupon.DeniedKey),
              getAreaNameDeclination(order.CuponDate.Area.Name),
              order.CuponDate.Area.Adres,
              order.CuponDate.Area.Phone);
        }
    }
}
