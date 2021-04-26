using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_D.Controllers
{
    public class PollController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        [Route("create_poll")]
        public IActionResult CreatePoll()
        {
            return View();
        }
    }
}
