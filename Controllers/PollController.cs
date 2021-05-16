using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjectD.Database;
using System;

namespace Project_D.Controllers
{
    public class PollController : Controller
    {

        public int QID;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string question, string answerA, string answerB)
        {
            if (question != null && answerA != null && answerB != null)
            {
                MySqlConnection connection;
                connection = new MySqlConnection(Connector.getString());
                try
                {
                    connection.Open();

                    string query = "SELECT id FROM polls ORDER BY id DESC LIMIT 0,1;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader;
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        QID = reader.GetInt32("id") + 1;
                    }
                    reader.Close();

                    query = $"INSERT INTO `database`.`polls` (`id`,`question`, `answerA`, `answerB`) VALUES ('{QID}','{question}','{answerA}','{answerB}');";

                    command = new MySqlCommand(query, connection);
                    reader = command.ExecuteReader();
                    reader.Close();
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

        [HttpPost]
        public IActionResult Vote(string Vote)
        {
            MySqlConnection connection;
            connection = new MySqlConnection(Connector.getString());
            try
            {
                connection.Open();

                string query = $"UPDATE `database`.`polls` SET `{Vote}` = `{Vote}` + 1, `totalVotes` = `totalVotes` + 1 WHERE id = 5;";

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
            return View();
        }
    }
}
