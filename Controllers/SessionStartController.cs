using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ProjectD.Models;
using Microsoft.AspNetCore.Mvc;

namespace ProjectD.Controllers
{
    public class SessionStartController : Controller
    { 
        
        public IActionResult Index()
        {
            return View();
        }

		[HttpGet]
		public IActionResult Index(string SessionCode)
		{
			if (SessionCode == null)
			{
				ViewBag.SessionMessage = string.Format("No session code was posted.");
			}
			else
			{
				ViewBag.SessionMessage = string.Format("Session code: {0}", SessionCode);
			}
			return View();
		}
	}
}