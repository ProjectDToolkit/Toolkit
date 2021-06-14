using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using ProjectD.Database;
using ProjectD.Models;
using System;
using System.Collections.Generic;
using static Project_D.Models.ResultModelcs;

namespace Project_D.Controllers
{
    public class PollController : Controller
    {

        public static int QID;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
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
        public IActionResult Index(string question, string answerA, string answerB)
        {
            if (AddPoll(HttpContext.Session.GetString("SessionCode"), question, answerA, answerB))
            {
                return RedirectToAction("QuestionsList", "Poll", new { sessionId = HttpContext.Session.GetString("SessionCode") });
            }
            else
		    {
                ViewBag.ErrorMessage = "Vul een alle velden in aub!";
                return View();
            }
        }

        /// <summary>
        /// function for creating a poll, puts the question with the answers in the database
        /// </summary>
        /// <param name="question">The poll question</param>
        /// <param name="answerA">The first answer</param>
        /// <param name="answerB">The second answer</param>
        /// <returns> returns bool true or false</returns>
        public static bool AddPoll(string sessioncode, string question, string answerA, string answerB)
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

                    query = $"INSERT INTO `database`.`polls` (`id`, `sessionId`,`question`, `answerA`, `answerB`) VALUES ('{QID}','{sessioncode}','{question}','{answerA}','{answerB}');";

                    command = new MySqlCommand(query, connection);
                    reader = command.ExecuteReader();
                    reader.Close();

                    return true;
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
            else
            {
                
                return false;
            }
        }
        /// <summary>
        /// Shows the list of all questions within the session
        /// </summary>
        /// <returns></returns>
        public IActionResult QuestionsList()
        {
            List<QuestionModel> questionsList = new List<QuestionModel>();

            MySqlConnection connection;
            connection = new MySqlConnection(Connector.getString());

            string sessioncode = HttpContext.Session.GetString("SessionCode");

            try
            {
                connection.Open();

                string query = $"SELECT * FROM database.polls WHERE sessionID = '{sessioncode}';";

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


            return View(questionsList);
        }

        /// <summary>
        /// Removes the selected poll from the database
        /// </summary>
        /// <param name="id">question ID for getting the right question in the DB</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult QuestionsList(int questionId)
        {
            if (DeleteQuestion(questionId))
			{
                return RedirectToAction("QuestionsList", "Poll");
            }
            else
			{
                // change this to error message
                return RedirectToAction("QuestionsList", "Poll");
            }

        }

        public static bool DeleteQuestion(int questionId)
		{
            MySqlConnection Connection;
            Connection = new MySqlConnection(Connector.getString());

            try
            {
                Connection.Open();
                string query = @"DELETE FROM database.polls WHERE id = @qid;";
                MySqlCommand cmd = new MySqlCommand(query, Connection);
                cmd.Parameters.AddWithValue("@qid", questionId);

                cmd.ExecuteNonQuery();

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                Connection.Close();
            }

            return true;
        }


        /// <summary>
        /// Get method for showing the view for the poll the user wants to vote on
        /// </summary>
        /// <param name="id">question ID for getting the right question in the DB</param>
        /// <returns></returns>
        public IActionResult Vote(int id)
        {
            QID = id;

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
                    ViewBag.Question = reader.GetString("question");
                    ViewBag.AnswerA = reader.GetString("answerA");
                    ViewBag.AnswerB = reader.GetString("answerB");
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

            return View(id);
        }

        /// <summary>
        /// Post method for updating total amount of votes when voting on a poll
        /// </summary>
        /// <param name="Vote">Gets the option that the user has voted on</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Vote(string Vote, int id = 0)
        {
                if (QID == 0)
             {
                    QID = id;
             }

                if (VoteOnQuestion(Vote))
			{
                return RedirectToAction("QuestionsList", "Poll", new { sessionId = HttpContext.Session.GetString("SessionCode") });
            }
            else
			{
                // change this to error message
                return RedirectToAction("QuestionsList", "Poll", new { sessionId = HttpContext.Session.GetString("SessionCode") });
            }
            
        }

        public static bool VoteOnQuestion(string Vote)
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
            return true;
        }
        
        public IActionResult Result(int id)
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
                    

                    dataPoints.Add(new DataPoint(answerA, votesA));

                    string answerB = reader.GetString("answerB");
                    int votesB = reader.GetInt32("votesB");
                    
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
    }
}
