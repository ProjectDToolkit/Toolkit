using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjectD.Database;
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
            if (question != null)
            {
                MySqlConnection connection;
                connection = new MySqlConnection(Connector.getString());
                try
                {
                    connection.Open();

                    string query = $"INSERT INTO `database`.`polls` (`question`, `answerA`, `answerB`) VALUES ('{question}','{answerA}','{answerB}');";

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

                return RedirectToAction("Vote", "Poll", new { Question = question, AnswerA = answerA, AnswerB = answerB });
            }
            else
            {
                ViewBag.ErrorMessage = "Vul een alle velden in aub!";
                return View();
            }
        }

        public IActionResult Vote(string question, string answerA, string answerB)
        {
            ViewBag.Question = question;
            ViewBag.AnswerA = answerA;
            ViewBag.AnswerB = answerB;
            return View();
        }
    }
}
