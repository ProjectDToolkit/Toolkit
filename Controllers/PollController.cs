using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Project_D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_D.Controllers
{
    public class PollController : Controller
    {

        MySqlConnection connection = new MySqlConnection(default);
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitPoll()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    connection.Open();

                    string q = $"INSERT INTO `polls`(`question`) VALUES (`hallo`)";

                    MySqlCommand comm = new MySqlCommand(q, connection);
                    MySqlDataReader reader = comm.ExecuteReader();
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
            return View();
        }
    }
}
