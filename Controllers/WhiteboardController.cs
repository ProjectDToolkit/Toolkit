﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectD.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project_D.Controllers
{
	public class WhiteboardController : Controller
	{
		// GET: /<controller>/
		public IActionResult Index()
		{
			return View();
		}

		public string Info = string.Format("");

	
	}
}
