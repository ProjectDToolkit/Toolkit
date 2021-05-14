using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ProjectD.Models;

namespace ProjectD.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

		[HttpPost]
		public IActionResult JoinSession(string sessionCode)
		{
            return RedirectToAction("Index", "SessionStart", new { SessionCode = sessionCode });
        }

		[HttpPost]
        public IActionResult Index(string newSessionCode)
        {
            return RedirectToAction("Index", "SessionStart", new { SessionCode = newSessionCode });
        }
    }
}