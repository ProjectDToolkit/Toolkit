using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjectD.Database;
using ProjectD.Models;
using static Project_D.Models.ResultModelcs;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project_D.Controllers
{
	public class WhiteboardController : Controller
	{
        public static int QID;

        // GET: /<controller>/
        public IActionResult Index(string Toggle = "Close")
		{
            List<QuestionModel> questionsList = new List<QuestionModel>();

            MySqlConnection connection;
            connection = new MySqlConnection(Connector.getString());

            try
            {
                connection.Open();

                string query = $"SELECT * FROM database.polls WHERE sessionID = '{HttpContext.Session.GetString("SessionCode")}';";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    bool answered = false;

                    if (reader.GetInt32("totalVotes") > 0)
                    {
                        answered = true;
                    }
                    var question = new QuestionModel
                    {
                        questionId = reader.GetInt32("id"),
                        SessionId = reader.GetString("sessionId"),
                        question = reader.GetString("question"),
                        isAnswered = answered

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

            if (Toggle == "Open") { ViewBag.Toggle = "Open"; } else { ViewBag.Toggle = "Closed"; }
			
            return View(questionsList);
        }

        [HttpGet]
        public IActionResult IndexWithNewPoll()
		{
            return View();
		}

        /// <summary>
        /// Post method for creating a poll, puts the question with the answers in the database
        /// </summary>
        /// <param name="question">The poll question</param>
        /// <param name="answerA">The first answer</param>
        /// <param name="answerB">The second answer</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult IndexWithNewPoll(string question, string answerA, string answerB)
        {
            if (PollController.AddPoll(HttpContext.Session.GetString("SessionCode"), question, answerA, answerB))
            {
                return RedirectToAction("Index", "Whiteboard");
            }
            else
            {
                ViewBag.ErrorMessage = "Vul een alle velden in aub!";
                return View();
            }
        }

        /// <summary>
        /// Get method for showing the view for the poll the user wants to vote on
        /// </summary>
        /// <param name="id">question ID for getting the right question in the DB</param>
        /// <returns></returns>
        public IActionResult IndexWithVoteOpen(int id)
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

        /// <summary>
        /// Post method for updating total amount of votes when voting on a poll
        /// </summary>
        /// <param name="Vote">Gets the option that the user has voted on</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult IndexWithVoteOpen(string Vote)
        {
            if (PollController.VoteOnQuestion(Vote))
            {
                return RedirectToAction("Index", "Whiteboard", new { sessionId = HttpContext.Session.GetString("SessionCode") });
            }
            else
            {
                // change this to error message
                return RedirectToAction("Index", "Whiteboard", new { sessionId = HttpContext.Session.GetString("SessionCode") });
            }
            
        }

        public IActionResult IndexWithResult(int id)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            MySqlConnection connection;
            connection = new MySqlConnection(Connector.getString());
            try
            {
                connection.Open();

                string query = $"SELECT * FROM database.polls WHERE id = '{id}';";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int totalvotes = reader.GetInt32("totalVotes");

                    string answerA = reader.GetString("answerA");
                    int votesA = reader.GetInt32("votesA");

                    float percA = votesA / totalvotes;

                    dataPoints.Add(new DataPoint(answerA, votesA));

                    string answerB = reader.GetString("answerB");
                    int votesB = reader.GetInt32("votesB");
                    int percB = (votesA / totalvotes) * 100;


                    dataPoints.Add(new DataPoint(answerB, votesB));

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

            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            return View();
        }

        /// <summary>
        /// Redirects to the right voting page for the poll the user has selected
        /// </summary>
        /// <param name="id">question ID for getting the right question in the DB</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeletePoll(int questionId)
        {
            if (PollController.DeleteQuestion(questionId))
            {
                return RedirectToAction("Index", "Whiteboard");
            }
            else
            {
                // change this to error message
                return RedirectToAction("Index", "Whiteboard");
            }
            
        }

    }

}

