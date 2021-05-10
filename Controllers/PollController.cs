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
        
        [Route("create_poll")]
        public IActionResult CreatePoll()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitPoll(PollModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    connection.Open();

                    string q = $"INSERT INTO `polls`(`question`) VALUES ({model.Question})";

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
