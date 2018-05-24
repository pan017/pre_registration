using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pre_registration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace pre_registration.Controllers
{
    public class BaseController : Controller
    {
        private string _username;

        protected string Username
        {
            get
            {
                if (_username == null)
                {
                    _username = User.Identity.Name;
                }
                return _username;
            }
        }

        string _currentUser;
        public BaseController(ApplicationContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            UserResolverService userResolverService = new UserResolverService(httpContextAccessor);
            _currentUser = userResolverService.GetUser();
            ViewBag.UserManager = userManager;
        }

    }
}
