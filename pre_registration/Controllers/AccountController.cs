﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pre_registration.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using pre_registration.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using pre_registration.Services;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Newtonsoft.Json;

namespace pre_registration.Controllers
{
    public class AccountController : Controller
    {
        ApplicationContext db;
        private IOptions<AppConfig> config;
        public AccountController(ApplicationContext context, IOptions<AppConfig> config)
        {
            db = context;
            this.config = config;
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
            if (!String.IsNullOrEmpty(model.Login) || !String.IsNullOrEmpty(model.Password))
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
                    if (Helpers.GetCulture(HttpContext) == "be")
                        ModelState.AddModelError("", "Няправільны лагін і (або) пароль");
                    else
                        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                    return View("LoginForm", model);
                }
            }
            if (Helpers.GetCulture(HttpContext) == "be")
                ModelState.AddModelError("", "Запоўніце ўсе палі");
            else
                ModelState.AddModelError("", "Заполните все поля");
            return View("LoginForm", model);
        }
        private async Task Authenticate(ApplicationUser user)
        {
            if (user.Role == null)
                user.Role = db.Roles.FirstOrDefault(x => x.Id == user.RoleId);
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
                if (Helpers.GetCulture(HttpContext) == "be")
                    ModelState.AddModelError("", "Памылка! Вы не прайшлі праверку бяспекі. Калі ласка, паспрабуйце яшчэ раз.");
                else
                    ModelState.AddModelError("", "Ошибка! Вы не прошли проверку безопасности. Пожалуйста, повторите ещё раз.");
            }
               
            if (String.IsNullOrEmpty(model.Login))
            {
                if (Helpers.GetCulture(HttpContext) == "be")
                    ModelState.AddModelError("", "Памылка! Калі ласка, увядзіце e-mail");
                else
                    ModelState.AddModelError("", "Ошибка! Введите e-mail");
            }

            if (String.IsNullOrEmpty(model.LastName))
            {
                if (Helpers.GetCulture(HttpContext) == "be")
                    ModelState.AddModelError("", "Памылка! Калі ласка, увядзіце прозвішча");
                else
                    ModelState.AddModelError("", "Ошибка! Введите фамилию");
            }

            if (String.IsNullOrEmpty(model.FirstName))
            {
                if (Helpers.GetCulture(HttpContext) == "be")
                    ModelState.AddModelError("", "Памылка! Увядзіце імя");
                else
                    ModelState.AddModelError("", "Ошибка! Введите имя");
            }
          
            if (String.IsNullOrEmpty(model.SecondName))
            {
                if (Helpers.GetCulture(HttpContext) == "be")
                    ModelState.AddModelError("", "Памылка! Увядзіце імя па бацьку");
                else
                    ModelState.AddModelError("", "Ошибка! Введите отчество");
            }
            
            if (String.IsNullOrEmpty(model.Phone))
            {
                if (Helpers.GetCulture(HttpContext) == "be")
                    ModelState.AddModelError("", "Памылка! Увядзіце нумар тэлефона");
                else
                    ModelState.AddModelError("", "Ошибка! Введите телефон");
            }
                 
            if (String.IsNullOrEmpty(model.Password) || String.IsNullOrEmpty(model.PasswordConfirm))
            {
                if (Helpers.GetCulture(HttpContext) == "be")
                    ModelState.AddModelError("", "Памылка! Увядзіце пароль");
                else
                    ModelState.AddModelError("", "Ошибка! Введите пароль");
            }
           
            if (!String.IsNullOrEmpty(model.Password) && !String.IsNullOrEmpty(model.PasswordConfirm) && model.PasswordConfirm != model.Password)
            {
                if (Helpers.GetCulture(HttpContext) == "be")
                    ModelState.AddModelError("", "Памылка! Паролі не супадаюць");
                else
                    ModelState.AddModelError("", "Ошибка! Пароли не совпадают");
            }
            
            if (ModelState.ErrorCount != 0)
            {
                return View(model);
            }
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
                EmailService.SendMail(config.Value.NotificationEmail, user.Login, "Подтверждение электронной почты",
                    $"" +
                    $"<div>" +
                    $"<H3>Здравсвуйте, {user.UserData.GetFullName()}</H3>" +
                    $"<p>Благодарим вас за регистрацию на {config.Value.WebSiteName}</p>" +
                    $"<p> Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>" +
                    $"</p>" +
                    $"</div>");
                return RedirectToAction("Index", "Home");

            }
            else
            {
                if (String.IsNullOrEmpty(model.Password) || String.IsNullOrEmpty(model.PasswordConfirm))
                {
                    if (Helpers.GetCulture(HttpContext) == "be")
                        ModelState.AddModelError("", "Карыстальнік з такім адрасам электроннай пошты ўжо існуе");
                    else
                        ModelState.AddModelError("", "Пользователь с таким адресом электронной почты уже существует");
                }
             
            }
            

            return View(model);
        }
        public async Task<IActionResult> ConfirmEmail( int userId, string code)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == userId && x.confirmKey == code);
            user.Role = db.Roles.FirstOrDefault(x => x.Id == user.RoleId);
            user.confirmedEmail = true;
            db.SaveChanges();
            await Authenticate(user);
            return RedirectToAction("Index", "Home");
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
                if (Helpers.GetCulture(HttpContext) == "be")
                    ModelState.AddModelError("", "Дадзеныя паспяхова захаваны!");
                else
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
                if (Helpers.GetCulture(HttpContext) == "be")
                    ModelState.AddModelError("", "Памылка! Стары пароль уведзены няправільна!");
                else
                    ModelState.AddModelError("", "Ошибка! Старый пароль введен не верно!");

                model.oldPassword = "";
                return RedirectToAction("Settings", model);
            }
            return RedirectToAction("Settings");
        }
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            ApplicationUser user = db.Users.Where(x => x.Login == model.Query).FirstOrDefault();
            if (user == null)
            {
                UserData userData = db.UsersData.FirstOrDefault(x => x.Phone.Trim().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") == model.Query.Trim().Replace("(","").Replace(")","").Replace("-", "").Replace(" ", ""));
                if (userData == null)
                {
                    if (Helpers.GetCulture(HttpContext) == "be")
                        ModelState.AddModelError("", "Карыстальнік не знойдзены! Праверце правільнасць ўводу дадзеных і паўтарыце спробу");
                    else
                        ModelState.AddModelError("", "Пользователь не найден! Проверьте правильность ввода данных и повторите попытку");

                    return View(model);
                }
                else
                {
                    user = db.Users.Where(x => x.UserDataID == userData.id).FirstOrDefault();
                }
            }
            user.UserData = db.UsersData.FirstOrDefault(x => x.id == user.UserDataID);
            pre_registration.Models.DataBaseModel.ResetPassword resetPassword = new Models.DataBaseModel.ResetPassword();
            resetPassword.ApplicationUser = user;
            resetPassword.ApplicationUserId = user.Id;
            resetPassword.UniqueKey = Helpers.ConvertStringtoMD5(user.Login + user.confirmKey + DateTime.Now.ToLongTimeString());
            db.ResetPasswords.Add(resetPassword);
            db.SaveChanges();
            var callbackUrl = Url.Action("ChangeResetPassword",
               "Account",
               new { confirmKey =resetPassword.UniqueKey},
               protocol: HttpContext.Request.Scheme);
            EmailService.SendMail(config.Value.NotificationEmail, user.Login, "Восстановление пароля",
                $"<b>Здравствуйте, {user.UserData.GetFullName()}</b> <br>" +
                $"Мы получили запрос на смену пароля от вашего аккаунта на  {config.Value.WebSiteName}" +               
                $"Чтобы его изменить, перейдите по ссылке " +
                $"<br> <a href='{callbackUrl}'>Изменить пароль</a> " +
                $"<p>Если вы не отправляли запрос, просто проигнорируйте это письмо. " +
                $"Ваш пароль не изменится, пока вы не перейдете по ссылке и не введете новые данные</p>");
            return View("ChangePasswordNotification");
        }

        public IActionResult ChangeResetPassword(string confirmKey)
        {
            var resetPasswordModel = db.ResetPasswords.FirstOrDefault(x => x.UniqueKey == confirmKey);
            if (resetPasswordModel == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ResetPasswordViewModel model = new ResetPasswordViewModel();
                model.Query = confirmKey;
                model.ResetPassword = resetPasswordModel;
                model.ResetPassword.ApplicationUser = db.Users.FirstOrDefault(x => x.Id == resetPasswordModel.ApplicationUserId);
                return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ChangeResetPassword(ResetPasswordViewModel model)
        {
            model.ResetPassword = db.ResetPasswords.FirstOrDefault(x => x.id == model.ResetPassword.id);
            var user = db.Users.FirstOrDefault(x => x.Id == model.ResetPassword.ApplicationUserId);
            user.Password = model.NewPassword;
            db.SaveChanges();
            var resetPasswordsCollection = db.ResetPasswords.Where(x => x.ApplicationUserId == user.Id);
            foreach (var item in resetPasswordsCollection)
            {
                db.ResetPasswords.Remove(item);
            }
            db.SaveChanges();
            await Authenticate(user);
            return RedirectToAction("Index", "Home");
        }
    }
}