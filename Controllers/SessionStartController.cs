using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ProjectD.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjectD.Database;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace ProjectD.Controllers
{
    public class SessionStartController : Controller
    { 
        
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
		/// Create a session, add the SessionCode to the Cookie named SessionCode
		/// </summary>
		/// <param name="hm">Home Model with paraneters</param>
		/// <returns></returns>
        [HttpPost]
        public IActionResult CreateSession(HomeModel hm)
		{
            HttpContext.Session.SetString("SessionCode", hm.SessionCode);

            if (HttpContext.Session.GetString("SessionCode") != null)
			{
                string sessionCode = HttpContext.Session.GetString("SessionCode");

                MySqlConnection Connection;
                Connection = new MySqlConnection(Connector.getString());
                try
                {
                    Connection.Open();
                    string stringToInsert = @"INSERT INTO sessions (sessionCode, peopleInSession) VALUES (@SessionCode, 1)";

                    using (MySqlCommand command = new MySqlCommand(stringToInsert, Connection))
                    {
                        // Add parameters here
                        command.Parameters.AddWithValue("@SessionCode", sessionCode);

                        command.Prepare();
                        int rowsUpdated = command.ExecuteNonQuery();

                        if (rowsUpdated > 0)
                        {
                            ViewBag.SessionMessage = string.Format("Session code: {0}, people online: 1", sessionCode);
                            ViewBag.SessionCode = string.Format(sessionCode);
                        }
                        else
                        {
                            ViewBag.SessionMessage = string.Format("Error inserting");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    Connection.Close();
                }
            }

            else
			{
                ViewBag.SessionMessage = string.Format("Error retreiving session");
            }
            
            return View("Index");
        }

        /// <summary>
		/// Create a session, add the JoinSessionCode to the Cookie named SessionCode
		/// </summary>
		/// <param name="hm">Home Model with paraneters</param>
		/// <returns></returns>
        [HttpPost]
        public IActionResult JoinSession(HomeModel hm)
		{
            if (DoesSessionExist(hm))
            {
                HttpContext.Session.SetString("SessionCode", hm.SessionCode);

                if (HttpContext.Session.GetString("SessionCode") != null)
                {
                    string sessionCode = HttpContext.Session.GetString("SessionCode");

                    MySqlConnection Connection;
                    Connection = new MySqlConnection(Connector.getString());
                    Connection.Open();
                    try
                    {
                        // updating the people online
                        string stringToInsert = @"UPDATE sessions SET peopleInSession = peopleInSession + 1 WHERE sessionCode = @SessionCode;";

                        using (MySqlCommand command = new MySqlCommand(stringToInsert, Connection))
                        {
                            // Add parameters here
                            command.Parameters.AddWithValue("@SessionCode", sessionCode);

                            command.Prepare();
                            int rowsUpdated = command.ExecuteNonQuery();

                            if (rowsUpdated > 0)
                            {
                                ViewBag.SessionMessage = string.Format("Session code: {0}, People online: {1}", sessionCode, GetPeopleInSession(sessionCode).ToString());
                                ViewBag.SessionCode = string.Format(sessionCode);
                            }
                            else
                            {
                                ViewBag.SessionMessage = string.Format("Error updating people online");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        Connection.Close();
                    }
                }
                else
                {
                    ViewBag.SessionMessage = string.Format("Error retreiving session");
                }
                return View("Index");
            }
            else
			{
                return RedirectToAction("Index", "Home", new { message = "Session Code not found" });
            }
        }
                     

        public IActionResult LeaveSession(string SessionCode)
		{
            MySqlConnection Connection;
            Connection = new MySqlConnection(Connector.getString());
            Connection.Open();
            try
            {
                // updating the people online
                string stringToInsert = @"UPDATE sessions SET peopleInSession = peopleInSession - 1 WHERE sessionCode = @SessionCode;";

                using (MySqlCommand command = new MySqlCommand(stringToInsert, Connection))
                {
                    // Add parameters here
                    command.Parameters.AddWithValue("@SessionCode", SessionCode);

                    command.Prepare();
                    int rowsUpdated = command.ExecuteNonQuery();

                    if (rowsUpdated > 0)
                    {
                        return RedirectToAction("Index", "Home", new { message = "Session Left" });

                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { message = "Error while leaving session" });
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Connection.Close();
            }
        }

        /// <summary>
		/// Checks if a session code exists in the database
		/// </summary>
		/// <param name="hm">home model with session code</param>
		/// <returns></returns>
        public bool DoesSessionExist(HomeModel hm)
		{
            MySqlConnection Connection;
            Connection = new MySqlConnection(Connector.getString());

            try
            {
                Connection.Open();
                string stringToRead = @"SELECT DISTINCT sessionCode FROM database.sessions where sessionCode = @SessionCode";
                MySqlCommand command = new MySqlCommand(stringToRead, Connection);

                // Add parameters here
                command.Parameters.AddWithValue("@SessionCode", hm.SessionCode);

                MySqlDataReader reader = command.ExecuteReader();
                if (!reader.HasRows)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Connection.Close();
            }
        }
    

        /// <summary>
		/// return int with amount of people currently in the session
		/// </summary>
		/// <param name="sessionCode">the current session code</param>
		/// <returns></returns>
		public int GetPeopleInSession(string sessionCode)
		{
            int peopleOnline = 0;

            MySqlConnection Connection;
            Connection = new MySqlConnection(Connector.getString());

            try
            {
                Connection.Open();
                string stringToRead = @"SELECT DISTINCT sessionCode, peopleInSession FROM database.sessions WHERE sessionCode = @SessionCode";
                MySqlCommand command = new MySqlCommand(stringToRead, Connection);

                // Add parameters here
                command.Parameters.AddWithValue("@SessionCode", sessionCode);


                using (MySqlDataReader myReader = command.ExecuteReader())
                {
                    myReader.Read();

                    // update int with peopleonline and close the db
                    peopleOnline = Int32.Parse(myReader["peopleInSession"].ToString());
                    Connection.Close();

                    //return current amount of people in the session
                    return peopleOnline;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Connection.Close();
            }
        }
	}
}