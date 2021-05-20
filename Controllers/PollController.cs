using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjectD.Database;
using ProjectD.Models;
using System;
using System.Collections.Generic;

namespace Project_D.Controllers
{
    public class PollController : Controller
    {

        public static int QID;
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

                return RedirectToAction("QuestionsList", "Poll", new { sessionId = "TEST"});
            }
            else
            {
                ViewBag.ErrorMessage = "Vul een alle velden in aub!";
                return View();
            }
        }

        public IActionResult QuestionsList(string sessionID)
        {
            sessionID = "TEST";
            List<QuestionModel> questionsList = new List<QuestionModel>();

            MySqlConnection connection;
            connection = new MySqlConnection(Connector.getString());

            try
            {
                connection.Open();

                string query = $"SELECT * FROM database.polls WHERE sessionID = '{sessionID}';";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var question = new QuestionModel
                    {
                        questionId = reader.GetInt32("id"),
                        SessionId = reader.GetString("sessionId"),
                        question = reader.GetString("question")
                    };
                    questionsList.Add(question);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                connection.Close();
            }


            return View(questionsList);
        }

        [HttpPost]
        public IActionResult QuestionsList(int id)
        {
            return RedirectToAction("Vote", "Poll", new { ID = id });
        }

        public IActionResult Vote(int id)
        {
            string question = "";
            string answerA = "";
            string answerB = "";

            MySqlConnection connection;
            connection = new MySqlConnection(Connector.getString());
            try
            {
                connection.Open();

                string query = $"SELECT * FROM polls WHERE `id` = '{id}';";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    question = reader.GetString("question");
                    answerA = reader.GetString("answerA");
                    answerB = reader.GetString("answerB");
                }
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

            ViewBag.Question = question;
            ViewBag.AnswerA = answerA;
            ViewBag.AnswerB = answerB;

            return View(id);
        }

        [HttpPost]
        public IActionResult Vote(string Vote)
        {
            MySqlConnection connection;
            connection = new MySqlConnection(Connector.getString());
            try
            {
                connection.Open();

                string query = $"UPDATE `database`.`polls` SET `{Vote}` = `{Vote}` + 1, `totalVotes` = `totalVotes` + 1 WHERE id = '{QID}';";

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
            return RedirectToAction("Index", "SessionStart", new { JoinSessionCode = "TEST"});
        }
    }
}
