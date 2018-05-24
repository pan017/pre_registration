using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pre_registration.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using pre_registration.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using pre_registration.Services;

namespace pre_registration.Controllers
{
    public class AccountController : Controller
    {
       // private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly UserManager<ApplicationUser> _userManager;
        ApplicationContext db;

        public AccountController(ApplicationContext context)
        {
            db = context;
        }
        public IActionResult Login(string returnUrl = null)
        {
            LoginViewModel model = new LoginViewModel { ReturnUrl = returnUrl };
            return PartialView(model);
        }
        public IActionResult LoginOrContinue()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user = db.Users.FirstOrDefault(x => x.Login == model.Login && x.Password == model.Password);
                if (user != null)
                {
                    user.Role = db.Roles.FirstOrDefault(x => x.Id == user.RoleId);
                    await Authenticate(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                    return View("LoginForm", model);
                }
            }
            
            return View(model);
        }
        private async Task Authenticate(ApplicationUser user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType , user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration(AddUserViewModel model)
        {
            ApplicationUser user = db.Users.FirstOrDefault(x => x.Login == model.Login);
            if (user == null)
            {
                model.RoleId = db.Roles.FirstOrDefault(x => x.Name == "user").Id;
                
                user = UserService.CreateUser(model, db);
                
                await Authenticate(user);

                var callbackUrl = Url.Action("ConfirmEmail",
                "Account", 
                new { userId = user.Id, code = user.confirmKey }, 
                protocol: HttpContext.Request.Scheme);
                EmailService emailService = new EmailService();
                emailService.SendMail(user.Login, "Подтверждение электронной почты",
                    $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction("Index", "Home");

            }
            else
            {
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            

            return View(model);
        }
        public async Task<IActionResult> ConfirmEmail( int userId, string code)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == userId && x.confirmKey == code);
            user.confirmedEmail = true;
            await Authenticate(user);
            return View();
        }
        public IActionResult continueWithOutRegistration()
        {
            HttpContext.Session.SetString("continueWithOutRegistration", "true");
            return RedirectToAction("Index", "Home");
        }

        public ApplicationUser GetCurrentUser()
        {
            var user = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);
            user.UserData = db.UsersData.FirstOrDefault(x => x.id == user.UserDataID);
            user.UserSettings = db.UserSettings.FirstOrDefault(x => x.id == user.UserSettingsId);
            user.Role = db.Roles.FirstOrDefault(x => x.Id == user.RoleId);
            
            return user;
        }
        [Authorize]
        public ActionResult Settings()
        {
            var user = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);
            user.UserData = db.UsersData.FirstOrDefault(x => x.id == user.UserDataID);
            user.UserSettings = db.UserSettings.FirstOrDefault(x => x.id == user.UserSettingsId);
            SettingsViewModel model = new SettingsViewModel
            {
                id = user.Id,
                Login = user.Login,
                FirstName = user.UserData.FirstName,
                SecondName = user.UserData.SecondName,
                LastName = user.UserData.LastName,
                Phone = user.UserData.Phone,
                SendEmail = user.UserSettings.SendEmail,
                SendReminder = user.UserSettings.SendReminder
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Settings(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(x => x.Id == model.id);
                user.UserData = db.UsersData.FirstOrDefault(x => x.id == user.UserDataID);
                user.UserSettings = db.UserSettings.FirstOrDefault(x => x.id == user.UserSettingsId);
                user.UserData.LastName = model.LastName;
                user.UserData.SecondName = model.SecondName;
                user.UserData.FirstName = model.FirstName;
                user.UserData.Phone = model.Phone;
                user.UserSettings.SendEmail = model.SendEmail;
                user.UserSettings.SendReminder = model.SendReminder;
                db.SaveChanges();
                ModelState.AddModelError("", "Данные успешно сохранены!");
                return View(model);
            }
            else
                return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            ChangePasswordModel model = new ChangePasswordModel();
            var user = GetCurrentUser();
            model.id = user.Id;
            return View(model);
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {

            ApplicationUser user = GetCurrentUser();
            if (user.Password == model.oldPassword)
            {
                user.Password = model.newPassword;
                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "Ошибка! Старый пароль введен не верно!");
                model.oldPassword = "";
                return RedirectToAction("Settings", model);
            }
            return RedirectToAction("Settings");
        }

    }
}