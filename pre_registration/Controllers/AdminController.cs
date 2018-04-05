using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using pre_registration.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using pre_registration.Models.ViewModels;

namespace pre_registration.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
      //  private readonly RoleManager<IdentityRole> _roleManager;
        private ApplicationContext db;

        public AdminController(UserManager<User> userManager,  SignInManager<User> signInManager, ApplicationContext context) //RoleManager<IdentityRole> roleManager,
        {
            db = context;
            _userManager = userManager;
            _signInManager = signInManager;
          //  _roleManager = roleManager;
          //  _roleManager.CreateAsync(new IdentityRole("Пользователь"));
            

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddNewUser()
        {
      //      ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.Areas = db.Areas.ToList();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UsersList()
        {
            List<UsersListViewModel> usersList = new List<UsersListViewModel>();
            foreach (var User in _userManager.Users)
            {
                UsersListViewModel viewModel = new UsersListViewModel();
                viewModel.Login = User.UserName;
                //viewModel.Id = User.Id;
              //  viewModel.Name = User.Name;
              //  viewModel.Area = User.Area.Name;
                viewModel.Area = db.Areas.First(x => x.Id == User.AreaId).Name;
                IList<string> a = await _userManager.GetRolesAsync(User);
                viewModel.Role = a.FirstOrDefault();            
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
            User user = await _userManager.FindByIdAsync(id);
            AddUserViewModel model = new AddUserViewModel();// = new AddUserViewModel({ AreaID = user.Area.Id, Login = user.UserName, Name = user.Name });
          //  model.AreaID = user.AreaId.GetValueOrDefault();
            model.Login = user.UserName;
          //  model.Name = user.Name;
            ViewBag.UserId = id;
      //      ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.Areas = db.Areas.ToList();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(AddUserViewModel model, string id)
        {
            User user = await _userManager.FindByIdAsync(id);
           // user.Name = model.Name;
            user.UserName = model.Login;
            user.PhoneNumber = model.Phone;
            user.Area = db.Areas.First(x => x.Id == model.AreaID);
            db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();
            
            return RedirectToAction("UsersList");
        }
        [HttpPost]
        public async Task<IActionResult> AddNewUser(AddUserViewModel model)
        {
            //await _roleManager.CreateAsync(new IdentityRole("Пользователь"));
            //if (ModelState.IsValid)
            //{
                User user = new User();
              //  user.Name = model.Name;
             //   user.UserName = model.Login;
                user.Area = db.Areas.First(x => x.Id == model.AreaID);
                var result = await _userManager.CreateAsync(user, model.Password);
                //await _userManager.AddToRoleAsync(user, _roleManager.Roles.First(x => x.Id == model.RoleId).Name);
                if (result.Succeeded)
                {
                    
                    ModelState.AddModelError(string.Empty, "Пользователь добавлен!");
                    model = new AddUserViewModel();
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                
            //}
        //    ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.Areas = db.Areas.ToList();
            return View(model);
        }
        public IActionResult ChangeUserPassword(string id)
        {
            ChangeUserPasswordModel model = new ChangeUserPasswordModel();
            if (_userManager.FindByIdAsync(id) != null)
            {
                model.id = id;
                model.userName = _userManager.FindByIdAsync(id).Result.UserName;
            }
            return PartialView(model);
        }
        [HttpPost]
        public IActionResult ChangeUserPassword(ChangeUserPasswordModel model)
        {
            var user = _userManager.FindByIdAsync(model.id).Result;
            _userManager.RemovePasswordAsync(user);
            _userManager.AddPasswordAsync(user, model.newPassword);
            ModelState.AddModelError(string.Empty, "Роль добавлена!");
            return RedirectToAction("UsersList");
        }
        //public ActionResult RolesList()
        //{
        //    ViewBag.UserManager = _userManager;
        //    return View(_roleManager.Roles.ToList());
        //}

        public ActionResult CreateRole()
        {
            return PartialView();
        }
        //[HttpPost]
        //public async Task<ActionResult> CreateRole(string name)
        //{
        //    if (ModelState.IsValid)
        //    {
        //       // IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
        //        if (result.Succeeded)
        //        {
        //            ModelState.AddModelError(string.Empty, "Роль добавлена!");
        //            return RedirectToAction("RolesList");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Что-то пошло не так");
        //        }
        //    }
        //    return View(name);
        //}

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