using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace pre_registration.Controllers
{
    public class ChiefController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}