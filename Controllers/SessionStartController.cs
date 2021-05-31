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
using Project_D.Shared;

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
            HttpContext.Session.SetString("UserCode", hm.UserCode);
            HttpContext.Session.SetString("WhiteboardSessionCode", hm.WhiteboardSessionCode);

            if (HttpContext.Session.GetString("SessionCode") != null)
			{
                string sessionCode = HttpContext.Session.GetString("SessionCode");
                string userCode = HttpContext.Session.GetString("UserCode");
                string whiteboardSessionCode = HttpContext.Session.GetString("WhiteboardSessionCode");

                MySqlConnection Connection;
                Connection = new MySqlConnection(Connector.getString());
                try
                {
                    Connection.Open();
                    string stringToInsert = @"INSERT INTO sessions (sessionCode, whiteboardSessionCode) VALUES (@SessionCode, @WhiteboardSessionCode)";

                    using (MySqlCommand command = new MySqlCommand(stringToInsert, Connection))
                    {
                        // Add parameters here
                        command.Parameters.AddWithValue("@SessionCode", sessionCode);
                        command.Parameters.AddWithValue("@WhiteboardSessionCode", whiteboardSessionCode);

                        command.Prepare();
                        int rowsUpdated = command.ExecuteNonQuery();

                        if (rowsUpdated > 0)
                        {
                            // returns a boolean false if usercode is not added yet
                            if (!Shared.IsUserCodeInSession(userCode, sessionCode))
							{
                                // returns a boolean false if code couldnt be added
                                if (!Shared.AddUserCodeToSession(userCode, sessionCode))
								{
                                    ViewBag.SessionMessage = string.Format("Error inserting usercode into session");
                                }
                            }
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
            if (Shared.DoesSessionExist(hm.SessionCode))
            {
                HttpContext.Session.SetString("SessionCode", hm.SessionCode);
                HttpContext.Session.SetString("UserCode", hm.UserCode);

                if (HttpContext.Session.GetString("SessionCode") != null)
                {
                    string sessionCode = HttpContext.Session.GetString("SessionCode");
                    string userCode = HttpContext.Session.GetString("UserCode");

                    // returns a boolean false if usercode is not added yet
                    if (!Shared.IsUserCodeInSession(userCode, sessionCode))
                    {
                        // returns a boolean false if code couldnt be added
                        if (!Shared.AddUserCodeToSession(userCode, sessionCode))
                        {
                            ViewBag.SessionMessage = string.Format("Error inserting usercode into session");
                        }
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
                     
       
        public IActionResult LeaveSession()
		{
            string sessionCode = HttpContext.Session.GetString("SessionCode");
            string userCode = HttpContext.Session.GetString("UserCode");
            // check if there's a session code in the session/cookies
            if (HttpContext.Session.GetString("SessionCode") != null)
            {

                // check if session exists in the db
                if (Shared.DoesSessionExist(sessionCode))
                {
                    // less than or equal to 1 people left in the session, delete the entire session
                    if (Shared.GetPeopleInSession(sessionCode) <= 1)
					{
                        // code to delete entire session
                        if (DeleteEntireSession(sessionCode))
						{
                            // code to delete user from session
                            if (DeleteUserFromSession(userCode, sessionCode))
                            {
                                return RedirectToAction("Index", "Home", new { message = "Session Left" });
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home", new { message = "Error while leaving session (could not delete user from session)" });
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home", new { message = "Error while leaving session (could not delete entire session)" });
                        }
                    }
                    else
					{
                        if (DeleteUserFromSession(userCode, sessionCode))
						{
                            return RedirectToAction("Index", "Home", new { message = "Session Left" });
                        }
                        else
						{
                            return RedirectToAction("Index", "Home", new { message = "Error while leaving session (could not delete user from session)" });
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { message = "Error while leaving session (sessioncode does not exist in db)" });
                }
            }
            else
            {
                return RedirectToAction("Index", "Home", new { message = "Error while leaving session (sessioncode does not exist in session)" });
            }
        }

        public bool DeleteEntireSession(string sessionCode)
        {
            MySqlConnection Connection;
            Connection = new MySqlConnection(Connector.getString());
            Connection.Open();
            try
            {
                string stringToInsert = @"DELETE FROM database.sessions WHERE sessionCode = @SessionCode;";

                using (MySqlCommand command = new MySqlCommand(stringToInsert, Connection))
                {
                    // Add parameters here
                    command.Parameters.AddWithValue("@SessionCode", sessionCode);

                    command.Prepare();
                    int rowsUpdated = command.ExecuteNonQuery();

                    if (rowsUpdated > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
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

        public bool DeleteUserFromSession(string userCode, string sessionCode)
		{
            MySqlConnection Connection;
            Connection = new MySqlConnection(Connector.getString());
            Connection.Open();
            try
            {
                string stringToInsert = @"DELETE FROM database.usersinsession WHERE usercode = @UserCode AND sessioncode = @SessionCode;";

                using (MySqlCommand command = new MySqlCommand(stringToInsert, Connection))
                {
                    // Add parameters here
                    command.Parameters.AddWithValue("@UserCode", userCode);
                    command.Parameters.AddWithValue("@SessionCode", sessionCode);

                    command.Prepare();
                    int rowsUpdated = command.ExecuteNonQuery();

                    if (rowsUpdated > 0)
                    {
                        return true;

                    }
                    else
                    {
                        return false;
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
    }
}