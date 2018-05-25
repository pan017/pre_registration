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
        string _currentUser;

        private readonly IOptions<AppConfig> config;
        public HomeController(ApplicationContext context, IHttpContextAccessor httpContextAccessor, IOptions<AppConfig> config)//, IOptions<NotificationEmailModel> config)
        {
            db = context;
            this.config = config;
            UserResolverService userResolverService = new UserResolverService(httpContextAccessor);
            _currentUser = userResolverService.GetUser();
          //  var a = db.Users.FirstOrDefault(x => x.UserName == _currentUser).AccessLevel;
            //var currentUserId = _userManager.GetUserId(User);
          //  HttpContext.Session.SetInt32("userAccessLevel", db.Users.FirstOrDefault(x => x.UserName == _currentUser).AccessLevel);
          //  ViewBag.CurrentUserAccessLevel = db.Users.FirstOrDefault(x => x.UserName == _currentUser).AccessLevel;
           
        }

        public IActionResult returnToSelectArea()
        {
            HttpContext.Session.Remove("Area");
            return RedirectToAction("selectAreaForm");
        }
        public IActionResult returnToSelectDate()
        {
            HttpContext.Session.Remove("Date");
            return RedirectToAction("viewCalendar", "Cupon", new { areaId = HttpContext.Session.GetInt32("Area")});
        }
        public IActionResult returnToSelectTime()
        {
            HttpContext.Session.Remove("CuponId");
            return RedirectToAction("viewTime", "Cupon", new { selectedDay = DateTime.Parse(HttpContext.Session.GetString("Date")), areaId = HttpContext.Session.GetInt32("Area") });
        }
        public Area getSessionArea()
        {
            if (String.IsNullOrEmpty(HttpContext.Session.GetInt32("Area").ToString()))
                return new Area();
            else
                return db.Areas.FirstOrDefault(x => x.Id == HttpContext.Session.GetInt32("Area")); 
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
        public IActionResult Index()
        {
            
        //    var b = ConfigurationManager.GetSection("NotificationEmail");

          //  var a = Configuration.
         //   var a = config;
            //notificationEmailModel.Login = 
            //GetSection("NotificationEmail").Bind(notificationEmailModel);
            //var a = _userManager.GetUsersInRoleAsync("ПОЛЬЗОВАТЕЛЬ");
            //var b = _userManager.Users;
            //  var c = _roleManager.Roles;
            //      IConfigurationSection connStrings = Configuration.GetSection("ConnectionStrings");

            //db.Areas.Add(new Area() { Email = "cen@mgaon.by", Name = "Центральный", Phone = "+375177418596" });
            //db.Areas.Add(new Area() { Email = "zav@mgaon.by", Name = "Заводской", Phone = "+375177418596" });
            //db.SaveChanges();
            //DateTime beginDate = new DateTime(2018, 1, 30, 8, 0, 0);
            //DateTime endDate = beginDate.AddMonths(1);
            //for (int i = 0; i < 30; i++)
            //{
            //    while (beginDate.Hour <= 18)
            //    {
            //        db.CuponDates.Add(new CuponDate() { Area = db.Areas.First(), date = beginDate, Status = "0" });
            //        beginDate = beginDate.AddMinutes(30);
            //        db.SaveChanges();
            //    }
            //    beginDate = beginDate.AddHours(14);
            //}

            return View();
        }
        
        public IActionResult selectTime(string cuponId)
        {

            return PartialView();
        }
        public IActionResult selectAreaForm()
        {
            List<Area> AreasList = db.Areas.ToList();

            ViewBag.SelectedArea = HttpContext.Session.GetInt32("Area");
            DateTime selectedDate = new DateTime();
            DateTime.TryParse(HttpContext.Session.GetString("Date"), out selectedDate);
            ViewBag.selectedDate = selectedDate;
            SelectList areasSelectList = new SelectList(AreasList, "Id", "Name");
            ViewBag.AreasList = db.Areas.ToList(); //areasSelectList;
            
            return PartialView();
        }
        public IActionResult recordForm(string id)
        {
            if (User.Identity.IsAuthenticated)
            {

                int userId = 1;//int.Parse(_userManager.GetUserId(User));
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
        public IActionResult addRecord(Order model)
        {
            //string captchaResponse = HttpContext.Request.Form["g-Recaptcha-Response"];
            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("https://www.google.com");

            //var values = new List<KeyValuePair<string, string>>();
            //values.Add(new KeyValuePair<string, string>
            //("secret", "6LdwakIUAAAAAFnZmf_drdtNojJPIeNSSRH32krI"));
            //values.Add(new KeyValuePair<string, string>
            // ("response", captchaResponse));
            //FormUrlEncodedContent content = new FormUrlEncodedContent(values);
            //HttpResponseMessage response = client.PostAsync("/recaptcha/api/siteverify", content).Result;

            //string verificationResponse = response.Content.
            //ReadAsStringAsync().Result;
            //var res = JsonConvert.DeserializeObject<ReCaptchaValidationResult>(verificationResponse);
            //if (!res.Success)
            //{

            //}
            CuponDate cuponDate = db.CuponDates.First(x => x.id == int.Parse(HttpContext.Session.GetString("CuponId")));
            Order newOrder = new Order();

            newOrder.CuponDateId = db.CuponDates.First(x => x.id == int.Parse(HttpContext.Session.GetString("CuponId"))).id;
            newOrder.OrderDate = DateTime.Now;
            newOrder.Comment = model.Comment;
            if (User.Identity.IsAuthenticated)
            {
                int userId = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name).Id;
                Client userClient = db.Clients.Where(x => x.User.Id == userId).FirstOrDefault();

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
            
            string s = String.Format("<a href='{0}/Cupon/DeniedCupon?key={1}>Отменить</a>", Request.Host.Value, deniedCupon.DeniedKey);
            HttpContext.Session.Remove("Date");
            HttpContext.Session.Remove("Area");
            HttpContext.Session.Remove("CuponId");
            HttpContext.Session.Remove("continueWithOutRegistration");


            string messageBody = String.Format("<html><body><br><img src='http://www.cyberforum.ru/images/cyberforum_logo.jpg\' alt='Super Game!'>" +
                " <br>Вы получили это письмо, потому что вы зарегистрировались на http://www.supergame.ru или сменили e-mail в профиле. <br>{4}<br>{5}    <br>{6} <br>{7} " +
                "<br>Код активации:       {3}<br><br>Мы будем рады видеть Вас на нашем сайте и желаем Вам приятой игры!" +
                "</body></html>",
                
                newOrder.Client.UserData.FirstName,
                 newOrder.Client.UserData.SecondName,
                 newOrder.Client.UserData.LastName,

                 String.Format("http://{0}/Cupon/DeniedCupon?key={1}", Request.Host.Value, deniedCupon.DeniedKey),
                  newOrder.CuponDate.date.ToShortDateString(),
                newOrder.CuponDate.date.ToShortTimeString(),
                newOrder.CuponDate.Area.Adres,
                newOrder.CuponDate.Area.Phone
                );
            EmailService emailService = new EmailService();
            
            emailService.SendMail(newOrder.Client.UserData.EmailAdress, "Запись в службу 'Одно окно'", messageBody);

            //String.Format("<h2>Благодарим за запись</h2> Дата: {0} Время:{1} Адрес: {2} Телефон: {3} Отмена записи: {4} ",
            //    newOrder.CuponDate.date.ToShortDateString(),
            //    newOrder.CuponDate.date.ToShortTimeString(),
            //    newOrder.CuponDate.Area.Adres,
            //    newOrder.CuponDate.Area.Phone,
            //    String.Format("<a href='{0}/Cupon/DeniedCupon?key={1}>Отменить</a>", Request.Host.Value, deniedCupon.DeniedKey))
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
