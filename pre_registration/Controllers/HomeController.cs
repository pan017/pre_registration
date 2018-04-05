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

namespace pre_registration.Controllers
{
    public class HomeController : Controller
    {
        ApplicationContext db;
        private UserManager<User> _userManager;
        public HomeController(ApplicationContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            db = context;   
            
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
            //HttpContext.Session.Remove("")
            return RedirectToAction("Home");
        }
        public IActionResult Index()
        {
            
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
            ViewBag.AreasList = areasSelectList;
             return PartialView();
        }
        public IActionResult recordForm(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                
                var user =  _userManager.GetUserAsync(User);
               
                Client userClient = db.Clients.Where(x => x.UserId == user.Id).First();
                ViewBag.user = user;
               
            }
            
            HttpContext.Session.SetString("CuponId", id);
            return PartialView();
        }
        [HttpPost]
        public IActionResult addRecord(Client model)
        {
            string captchaResponse = HttpContext.Request.Form["g-Recaptcha-Response"];
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://www.google.com");

            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>
            ("secret", "6LdwakIUAAAAAFnZmf_drdtNojJPIeNSSRH32krI"));
            values.Add(new KeyValuePair<string, string>
             ("response", captchaResponse));
            FormUrlEncodedContent content = new FormUrlEncodedContent(values);
            HttpResponseMessage response = client.PostAsync("/recaptcha/api/siteverify", content).Result;
            
            string verificationResponse = response.Content.
            ReadAsStringAsync().Result;
            var res = JsonConvert.DeserializeObject<ReCaptchaValidationResult>(verificationResponse);
            if (!res.Success)
            {
                
            }
            CuponDate cuponDate = db.CuponDates.First(x => x.id == int.Parse(HttpContext.Session.GetString("CuponId")));
            //User user = new User { Email = model.User.Email, UserName = model.User.Email,  PhoneNumber = model.User.PhoneNumber }; //Name = model.User.Name,
            
            var userData = model.UserData;
            db.UsersData.Add(userData);
            db.SaveChanges();
            model.UserDataID = userData.id;
            // Client client = new Client();
            pre_registration.Models.Client newClient = new pre_registration.Models.Client();
            newClient.UserDataID = userData.id;
            newClient.Comment = model.Comment;
            if (model.UserId != null)
                newClient.UserId = model.UserId;
            
            db.Clients.Add(newClient);
            db.SaveChanges();

            cuponDate.ClientId = newClient.id;
            cuponDate.regDate = DateTime.Now;
            cuponDate.Status = "1";
            db.Entry(cuponDate).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();
            HttpContext.Session.Remove("Date");
            HttpContext.Session.Remove("Area");
            HttpContext.Session.Remove("CuponId");
            HttpContext.Session.Remove("continueWithOutRegistration");
            return View();
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
