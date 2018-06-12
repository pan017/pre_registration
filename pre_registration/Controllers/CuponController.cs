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
using pre_registration.Models.DataBaseModel;
using System.Security.Claims;

namespace pre_registration.Controllers
{
    public class CuponController : Controller
    {
        ApplicationContext db;
        //SignInManager<ApplicationUser> _signInManager;
        string _currentUser;
        public IActionResult Index()
        {
            return View();
        }
        public  CuponController (ApplicationContext context)
        {
            db = context;
      
        }
      
        public List<CuponDate> getFreeCupons(int areaId)
        {
            List<Order> orders = db.Orders.ToList();
            List<CuponDate> allCuponList = db.CuponDates.Where(x => x.Area.Id == areaId).ToList();
            List<CuponDate> busyCupons = new List<CuponDate>();


            foreach (var cupon in allCuponList)
            {
                foreach (var order in orders)
                {
                    if (cupon.id == order.CuponDateId)
                        busyCupons.Add(cupon);
                }
            }
            return allCuponList.Except(busyCupons).ToList();

        }
        public IActionResult viewCalendar(int areaId)
        { 
       
            //List<Order> orders = db.Orders.ToList();
            //List<CuponDate> allCuponList =  db.CuponDates.Where(x => x.Area.Id == areaId).ToList();
            //List<CuponDate> busyCupons = new List<CuponDate>();
            

            //    foreach (var cupon in allCuponList)
            //    {
            //        foreach (var order in orders)
            //        {
            //            if (cupon.id == order.CuponDateId)
            //                busyCupons.Add(cupon);
            //        }
            //    }
            List<CuponDate> model = getFreeCupons(areaId);//allCuponList.Except(busyCupons).ToList();
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
                List<CuponDate> model = getFreeCupons(areaId).Where(x => x.date.Date == selectedDay.Date).ToList();//db.CuponDates.Where(x => x.Area.Id == areaId && x.date.Date == selectedDay.Date).ToList();
                return PartialView(model);
            }
            else
            {
                return RedirectToAction("LoginOrContinue", "Account");
            }
        }
        public IActionResult viewRegisterCupons()
        {
            
            List<Order> orders;
            if (User.Identity.IsAuthenticated && User.IsInRole("superuser"))
            {
                var user = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);
                orders = db.Orders.Where(x => x.CuponDate.AreaId == user.AreaId.Value).ToList();
            }
            else
                orders = db.Orders.ToList();

            foreach (var item in orders)
            {
                item.Client = db.Clients.First(x => x.id == item.ClientId);
                item.CuponDate = db.CuponDates.First(x => x.id == item.CuponDateId);
                item.Client.UserData = db.UsersData.First(x => x.id == item.Client.UserDataID);
            }
            
            return View(orders);
            
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
            if (User.Identity.IsAuthenticated && User.IsInRole("superuser"))
            {
                var user = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);
                model.AreaId = user.AreaId.Value;
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

                        db.CuponDates.Add(new CuponDate() { Area = db.Areas.First(x => x.Id == model.AreaId), date = tempBeginTime });

                        tempBeginTime= tempBeginTime.AddMinutes(model.interval);


                    }
                tempDate = tempDate.AddDays(1);
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult DeniedCupon(string key)
        {
            DeniedCupon deniedCupon = db.DeniedCupons.FirstOrDefault(x => x.DeniedKey == key);
            if (deniedCupon != null)
            {
                Order order = db.Orders.FirstOrDefault(x => x.id == deniedCupon.OrderId);
                db.Orders.Remove(order);
                db.SaveChanges();
            }

            return RedirectToAction("MyCupons", "Cupon");
        }

        public ActionResult MyCupons()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);
                List<MyCuponsViewModel> model = new List<MyCuponsViewModel>();
                var orderList = db.Orders.Where(x => x.Client.UserId == user.Id).ToList();
                foreach (var item in orderList)
                {
                    item.Client = db.Clients.First(x => x.id == item.ClientId);
                    item.CuponDate = db.CuponDates.First(x => x.id == item.CuponDateId);
                    MyCuponsViewModel modelItem = new MyCuponsViewModel();
                    modelItem.id = item.id;
                    modelItem.orderDate = item.OrderDate.ToShortDateString();
                    modelItem.cuponDate = item.CuponDate.date.ToShortDateString();
                    modelItem.cuponTime = item.CuponDate.date.ToShortTimeString();
                    modelItem.Coment = item.Comment;
                    modelItem.DeniedKey = db.DeniedCupons.FirstOrDefault(x => x.OrderId == item.id).DeniedKey;
                    model.Add(modelItem);
                  //  item.Client.UserData = db.UsersData.First(x => x.id == item.Client.UserDataID);
                }
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}