using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pre_registration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace pre_registration.Controllers
{
    public class CuponController : Controller
    {
        ApplicationContext db;
        SignInManager<User> _signInManager;
        public IActionResult Index()
        {
            return View();
        }
        public  CuponController (ApplicationContext context, SignInManager<User> signInManager)
        {
            db = context;
            _signInManager = signInManager;
        }
        public IActionResult viewCalendar(int areaId)
        {
            List<CuponDate> model = db.CuponDates.Where(x => x.Area.Id == areaId && x.Status =="0").ToList();
            if (areaId != 0)
                HttpContext.Session.SetInt32("Area", areaId);
            ViewBag.SelectedArea = areaId;
            return PartialView(model);
        }
        public IActionResult viewTimeWithOutRegistration()
        {
            int? areaId = HttpContext.Session.GetInt32("Area");
            DateTime selectedDate = new DateTime();
            DateTime.TryParse(HttpContext.Session.GetString("Date"), out selectedDate);
            List <CuponDate> model = db.CuponDates.Where(x => x.Area.Id == areaId && x.date.Date == selectedDate.Date).ToList();
            return PartialView("viewTime", model);
        }
        public IActionResult viewTime(DateTime selectedDay, int areaId)
        {
            HttpContext.Session.SetString("Date", selectedDay.ToString("d"));
            var a = HttpContext.Session.GetString("continueWithOutRegistration");
            bool continueWithOutRegistration = false;
            continueWithOutRegistration = bool.TryParse(HttpContext.Session.GetString("continueWithOutRegistration"), out continueWithOutRegistration);
            if (User.Identity.IsAuthenticated || continueWithOutRegistration)
            {                 
                DateTime d = selectedDay.Date;
                List<CuponDate> model = db.CuponDates.Where(x => x.Area.Id == areaId && x.date.Date == selectedDay.Date).ToList();
                return PartialView(model);
            }
            else
            {
                return RedirectToAction("LoginOrContinue", "Account");
            }
        }
    }
}