using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
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

        [HttpPost]
        public IActionResult Index(string Question)
        {
            var connection = new MySqlConnection(default);

            try
            {
                connection.Open();

                string query = "INSERT INTO `database`.`polls` (`question`) VALUES ('Testing DatabaseConnection');";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader;
                reader = command.ExecuteReader();
            }
            catch (Exception)
            {

                
                throw;
            }
            finally
            {
                connection.Close();
            }

            
            return RedirectToAction("Index", "Home", new { question = Question });
        }
    }
}
