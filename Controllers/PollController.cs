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
        public IActionResult Index(string question, string answerA, string answerB)
        {
            if (question != null && answerA != null && answerB != null)
            {
                return RedirectToAction("Vote", "Poll", new { Question = question, AnswerA = answerA, AnswerB = answerB });
            }
            else
            {
                ViewBag.ErrorMessage = "Vul een alle velden in aub!";
                return View();
            }
            /*
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
            */

        }

        public IActionResult Vote()
        {
            return View();
        }
    }
}
