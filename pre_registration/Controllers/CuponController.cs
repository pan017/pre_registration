using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pre_registration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pre_registration.Models.ViewModels;

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
        public IActionResult viewRegisterCupons(int AreaId)
        {
            var model = db.CuponDates.Include(x => x.Area).Include(x => x.Client).Where(x => x.AreaId == AreaId && x.Status == "1").ToList(); 
            //List<CuponDate> model = db.CuponDates.Where(x => x.AreaId == AreaId && x.Status == "1").ToList();
            //db.Entry(model).References("Clients").Load();
            //foreach (var item in db.CuponDates.Where(x => x.AreaId == AreaId && x.Status == "1").ToList())
            //{
            //    //model.Add(item);
            //    CuponDate cuponDate = item;
            //    cuponDate.Client = db.Clients.First(x => x.id == item.ClientId);
            //    cuponDate.Area = db.Areas.First(x => x.Id == item.AreaId);
            //    model.Add(cuponDate);
            //}
            return View(model);
            
        }

        public IActionResult setCuponInterval()
        {
            SetCuponIntervalViewModel model = new SetCuponIntervalViewModel();
            model.daysOfWeek = new bool[7];
            ViewBag.Hours = new List<int>();
            ViewBag.Mins = new List<int>();
            for (int i = 0; i < 24; i++)
                ViewBag.Hours.Add(i + 1);
            for (int i = 0; i < 60; i++)
                ViewBag.Mins.Add(i);
            ViewBag.Areas = db.Areas.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult setCuponInterval(SetCuponIntervalViewModel model)
        {
            if (model.beginDate > model.endDate)
            {

                ModelState.AddModelError("", "Ошибка! Дата начала периода не может быть позже даты окончания");
                ViewBag.Hours = new List<int>();
                ViewBag.Mins = new List<int>();
                for (int i = 0; i < 24; i++)
                    ViewBag.Hours.Add(i + 1);
                for (int i = 0; i < 60; i++)
                    ViewBag.Mins.Add(i);
                ViewBag.Areas = db.Areas.ToList();
                return View(model);               
            }
            DateTime tempDate = model.beginDate;
            while (model.endDate >= tempDate)
            {
                DateTime tempBeginTime = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, model.beginTimeHours, model.beginTimeMins, 0);
                DateTime tempEndTime = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, model.endTimeHours, model.endTimeMins, 0);
                var a = (int)tempBeginTime.DayOfWeek;
                if (model.daysOfWeek[(int)tempBeginTime.DayOfWeek])
                    while (tempBeginTime <= tempEndTime)
                    {

                        db.CuponDates.Add(new CuponDate() { Area = db.Areas.First(), date = tempBeginTime, Status = "0" });

                        tempBeginTime= tempBeginTime.AddMinutes(model.interval);


                    }
                tempDate = tempDate.AddDays(1);
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}