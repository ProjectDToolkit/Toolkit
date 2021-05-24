using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MySql.Data.MySqlClient;
using ProjectD.Database;
using ProjectD.Models;
using Microsoft.AspNetCore.Http;


namespace ProjectD.Controllers
{
    public class HomeController : Controller
    {
        public string SessionCode;

        public IActionResult Index(string message = "")
        {
            ViewBag.Message = message;
            return View();
        }
    }
}