﻿using System;
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
using pre_registration.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace pre_registration.Controllers
{
    public class CuponController : Controller
    {
        ApplicationContext db;
        private readonly IOptions<AppConfig> config;
        public IActionResult Index()
        {
            return View();
        }
        public  CuponController (ApplicationContext context, IOptions<AppConfig> config)
        {
            db = context;
            this.config = config;
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
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            HttpContext.Session.SetString("Date", selectedDay.ToString("d"));
            var a = HttpContext.Session.GetString("continueWithOutRegistration");
            bool continueWithOutRegistration = false;
            continueWithOutRegistration = true; //bool.TryParse(HttpContext.Session.GetString("continueWithOutRegistration"), out continueWithOutRegistration);
            if (User.Identity.IsAuthenticated || continueWithOutRegistration)
            {                 
                DateTime d = selectedDay.Date;
                List<CuponDate> model = getFreeCupons(areaId).Where(x => x.date.Date == selectedDay.Date).ToList();//db.CuponDates.Where(x => x.Area.Id == areaId && x.date.Date == selectedDay.Date).ToList();
                model = model.OrderBy(x => x.date).ToList();
                return PartialView(model);
            }
            else
            {
                return RedirectToAction("LoginOrContinue", "Account");
            }
        }
        [Authorize(Roles = "admin, superuser")]
        public IActionResult viewRegisterCupons()
        {
            
            List<Order> orders;
            if (User.Identity.IsAuthenticated && User.IsInRole("superuser"))
            {
                var user = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);
                orders = db.Orders.Where(x => x.CuponDate.AreaId == user.AreaId.Value && x.CuponDate.date > DateTime.Now).ToList();
            }
            else
                orders = db.Orders.Where(x => x.CuponDate.date > DateTime.Now).ToList();

            foreach (var item in orders)
            {
                item.Client = db.Clients.First(x => x.id == item.ClientId);
                item.CuponDate = db.CuponDates.First(x => x.id == item.CuponDateId);
                item.Client.UserData = db.UsersData.First(x => x.id == item.Client.UserDataID);
                item.CuponDate.Area = db.Areas.FirstOrDefault(x => x.Id == item.CuponDate.AreaId);
            }
            orders = orders.OrderBy(x => x.CuponDate.date).ToList();
            return View(orders);
            
        }
        [Authorize(Roles = "admin, superuser")]
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
        [Authorize(Roles = "admin, superuser")]
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
          //  int interval = 0;
            //bool isCorrectInterval = int.TryParse(model.interval, interval);
          //  if (model.interval)
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
        [HttpGet]
        [Authorize(Roles = "admin, superuser")]
        public ActionResult RemoveCupons()
        {
            RemoveCuponsModel model = new RemoveCuponsModel();
            model.daysOfWeek = new bool[7];
            ViewBag.Areas = db.Areas.ToList();
            return View(model);
        }
        [Authorize(Roles = "admin, superuser")]
        public ActionResult RemoveCupons(RemoveCuponsModel model)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("superuser"))
            {
                var user = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);
                model.AreaId = user.AreaId.Value;
            }
            RemoveCuponsMethod(model);
            return RedirectToAction("Index", "Home");
        }
        void RemoveCuponsMethod(RemoveCuponsModel model)
        {
           
            bool isNotSelectedDays = true;
            for (int i = 0; i < model.daysOfWeek.Count(); i++)
            {
                if (model.daysOfWeek[i] == true)
                {
                    isNotSelectedDays = true;
                    break;
                }
            }
            List<CuponDate> removeList = new List<CuponDate>();
            DateTime tempDate = model.beginDate;
            while (model.endDate >= tempDate)
            {
                DateTime tempBeginTime = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0);
                DateTime tempEndTime = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 23, 59, 59);
                var a = (int)tempBeginTime.DayOfWeek;
                if (model.daysOfWeek[(int)tempBeginTime.DayOfWeek] || isNotSelectedDays)
                {
                    removeList.AddRange(db.CuponDates.Where(x => x.AreaId == model.AreaId && x.date > tempBeginTime && x.date < tempEndTime).ToList());

                }
                tempDate = tempDate.AddDays(1);
            }
            foreach (var item in removeList)
            {
                if (db.Orders.Where(x => x.CuponDateId == item.id).Count() > 0)
                    continue;
                db.CuponDates.Remove(item);
            }
            db.SaveChanges();
        }
        [HttpGet]
        [Authorize(Roles = "admin, superuser")]
        public bool RemoveDayCupons(string day)
        {
            DateTime selectDay;
            if (DateTime.TryParse(day, out selectDay))
            {
                RemoveCuponsModel model = new RemoveCuponsModel { beginDate = selectDay, endDate = selectDay, daysOfWeek = new bool[6] };

                if (User.Identity.IsAuthenticated && User.IsInRole("superuser"))
                {
                    var user = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);
                    model.AreaId = user.AreaId.Value;
                }
                if (User.Identity.IsAuthenticated && User.IsInRole("admin"))
                {
                    model.AreaId = HttpContext.Session.GetInt32("Area").Value;
                }
                RemoveCuponsMethod(model);

                return true;
            }
            return false;
        }
        public ActionResult DeniedCupon(string key)
        {
            DeniedCupon deniedCupon = db.DeniedCupons.FirstOrDefault(x => x.DeniedKey == key);
            if (deniedCupon != null)
            {
                Order order = db.Orders.FirstOrDefault(x => x.id == deniedCupon.OrderId);
                order.Client = db.Clients.FirstOrDefault(x => x.id == order.ClientId);
                order.Client.UserData = db.UsersData.FirstOrDefault(x => x.id == order.Client.UserDataID);
                order.CuponDate = db.CuponDates.FirstOrDefault(x => x.id == order.CuponDateId);
                order.CuponDate.Area = db.Areas.FirstOrDefault(x => x.Id == order.CuponDate.AreaId);
                EmailService.SendMail(config.Value.NotificationEmail, order.CuponDate.Area.NotificationEmail, "Отмена записи", getMessageBody(order));
                db.Orders.Remove(order);
                db.SaveChanges();
           
            }

            return RedirectToAction("MyCupons", "Cupon");
        }
        private string getMessageBody(Order order)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            return String.Format(@"
                 <html>
                <body>
                <br>
                    Доброго времени суток.
                    <br> Заявитель: {0} 
                    <br> Дата записи: {1}
                    <br> Время записи: {2}
                    <br> Телефон: {3}
                    <br> e-mail: {4}
                 </body>
                </html>
            
            ", order.Client.UserData.GetFullName(), 
            order.CuponDate.date.ToShortDateString(),
            order.CuponDate.date.ToLongTimeString(),

            order.Client.UserData.Phone,
            order.Client.UserData.EmailAdress);
        }
        [Authorize]
        public ActionResult MyCupons()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);
                List<MyCuponsViewModel> model = new List<MyCuponsViewModel>();
                var orderList = db.Orders.Where(x => x.Client.UserId == user.Id).OrderBy(x => x.CuponDate).ToList();
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