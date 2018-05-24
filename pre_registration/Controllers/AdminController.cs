using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using pre_registration.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using pre_registration.Models.ViewModels;
using pre_registration.Models.DataBaseModel;

namespace pre_registration.Controllers
{
    public class AdminController : Controller
    {
       // private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly RoleManager<ApplicationRole> _roleManager;
        private ApplicationContext db;

        public AdminController(ApplicationContext context) //RoleManager<IdentityRole> roleManager,
        {
            db = context;
            //_userManager = userManager;
            //_signInManager = signInManager;
            //_roleManager = roleManager;
          //  _roleManager.CreateAsync(new IdentityRole("Пользователь"));
            

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddNewUser()
        {
            ViewBag.Roles = db.Roles.ToList();
            ViewBag.Areas = db.Areas.ToList();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UsersList()
        {
            List<UsersListViewModel> usersList = new List<UsersListViewModel>();
            foreach (var item in db.Users.ToList())
            {
                UsersListViewModel viewModel = new UsersListViewModel();
                var userData = db.UsersData.FirstOrDefault(x => x.id == item.UserDataID);
                var area = db.Areas.FirstOrDefault(x => x.Id == item.AreaId);
                var userRole = db.Roles.FirstOrDefault(x => x.Id == item.RoleId);
                viewModel.Login = item.Login;
                viewModel.Id = item.Id.ToString();
                viewModel.Name = String.Format("{0} {1} {2}", userData.FirstName, userData.LastName, userData.SecondName);
                viewModel.Area = area == null ? "" : area.Name;
                viewModel.Role = userRole == null ? "" : userRole.Name;
                usersList.Add(viewModel);
            }
            return View(usersList);
        }


        [HttpGet]
        public IActionResult AddArea()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddArea(Area model)
        {
            if (ModelState.IsValid)
            {
                var result = db.Areas.AddAsync(model);
                await db.SaveChangesAsync();
                if (result.IsCompletedSuccessfully)
                {
                   
                    ModelState.AddModelError(string.Empty, "Район добавлен!");
                    model = new Area();
                   
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Exception.Message);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> EditUser(string id)
        {
            ApplicationUser user = db.Users.FirstOrDefault(x => x.Id == int.Parse(id));
            AddUserViewModel model = new AddUserViewModel();// = new AddUserViewModel({ AreaID = user.Area.Id, Login = user.UserName, Name = user.Name });
                                                            //  model.AreaID = user.AreaId.GetValueOrDefault();
            model.Login = user.Login;
            UserData userData = db.UsersData.FirstOrDefault(x => x.id == user.UserDataID);
            model.LastName = userData.LastName;
            model.FirstName = userData.FirstName;
            model.SecondName = userData.SecondName;
            model.Phone = userData.Phone;
            model.RoleId = user.RoleId.Value;
            model.AreaID = user.AreaId.GetValueOrDefault();
            ViewBag.roles = new SelectList(db.Roles, "Id", "Name", user.RoleId);
            if (user.AreaId == null)
            {
                ViewBag.areas = new SelectList(db.Areas, "Id", "Name");
            }
            else
                ViewBag.areas = new SelectList(db.Areas, "Id", "Name", user.AreaId);

            ViewBag.UserId = id;
                 ViewBag.Roles = db.Roles.ToList();
            ViewBag.Areas = db.Areas.ToList();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(AddUserViewModel model, int id)
        {
            ApplicationUser user = db.Users.FirstOrDefault(x => x.Id == id);
            UserData userData = db.UsersData.FirstOrDefault(x => x.id == user.UserDataID);
            user.RoleId = model.RoleId;
            user.AreaId = model.AreaID;

            userData.LastName = model.LastName;
            userData.FirstName = model.FirstName;
            userData.SecondName = model.SecondName;
            userData.Phone = model.Phone;
            db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("UsersList");
        }
        [HttpPost]
        public IActionResult AddNewUser(AddUserViewModel model)
        {
            
            UserService.CreateUser(model, db);
            ViewBag.Roles = db.Roles.ToList();
            ViewBag.Areas = db.Areas.ToList();
            return View(model);
        }
        //public IActionResult ChangeUserPassword(string id)
        //{
        //    ChangeUserPasswordModel model = new ChangeUserPasswordModel();
        //    if (_userManager.FindByIdAsync(id) != null)
        //    {
        //        model.id = id;
        //        model.userName = _userManager.FindByIdAsync(id).Result.Login;
        //    }
        //    return PartialView(model);
        //}
        //[HttpPost]
        //public IActionResult ChangeUserPassword(ChangeUserPasswordModel model)
        //{
        //    var user = _userManager.FindByIdAsync(model.id).Result;
        //    _userManager.RemovePasswordAsync(user);
        //    _userManager.AddPasswordAsync(user, model.newPassword);
        //    ModelState.AddModelError(string.Empty, "Роль добавлена!");
        //    return RedirectToAction("UsersList");
        //}
        //public ActionResult RolesList()
        //{
        //    var a = _userManager;

        //    // var b = _roleManager;
        //    ViewBag.UserManager = _userManager;

        //    return View(_roleManager.Roles.ToList());
        //}

        public ActionResult CreateRole()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult CreateRole(string name)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(new ApplicationRole(name));
                db.SaveChanges();

                    ModelState.AddModelError(string.Empty, "Роль добавлена!");
                    return RedirectToAction("RolesList");
                
            }
            return View(name);
        }
        public ActionResult RolesList()
        {
            return View(db.Roles.ToList());
        }
        //public async Task<ActionResult> EditRole(string id)
        //{
        //    ApplicationRole role = await _roleManager.FindByIdAsync(id);
        //    if (role != null)
        //    {
        //        return View(new EditRoleModel { Id = role.Id, Name = role.Name, Description = role.Description });
        //    }
        //    return RedirectToAction("Index");
        //}
        //[HttpPost]
        //public async Task<ActionResult> EditRole(EditRoleModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ApplicationRole role = await _roleManager.FindByIdAsync(model.Id);
        //        if (role != null)
        //        {
        //            role.Description = model.Description;
        //            role.Name = model.Name;
        //            IdentityResult result = await _roleManager.UpdateAsync(role);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Index");
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "Что-то пошло не так");
        //            }
        //        }
        //    }
        //    return View(model);
        //}

        //public async Task<ActionResult> DeleteRole(string id)
        //{
        //    //var role = await _roleManager.FindByIdAsync(id);
        //    if (role != null)
        //    {
        //  //      IdentityResult result = await _roleManager.DeleteAsync(role);
        //    }

        //    ModelState.AddModelError(string.Empty, "Роль удалена!");
        //    return RedirectToAction("RolesList");
        //}
    }
}