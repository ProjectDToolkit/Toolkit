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

namespace ProjectD.Controllers
{
    public class SessionStartController : Controller
    { 
        
        public IActionResult Index()
        {
            return View();
        }

		[HttpGet]
		public IActionResult Index(string CreateSessionCode = "", string JoinSessionCode = "")
        {
            // Neither have a value, error
            if (CreateSessionCode == "" && JoinSessionCode == "")
			{
				ViewBag.SessionMessage = string.Format("No session code was posted.");
			}

            // Both have a value, error
            else if (CreateSessionCode != "" && JoinSessionCode != "")
            {
                ViewBag.SessionMessage = string.Format("2 session codes were posted.");
            }

            // CreateSessionCode has a value, Create a session
            else if (CreateSessionCode != "" && JoinSessionCode == "")
            {
                MySqlConnection Connection;
                Connection = new MySqlConnection(Connector.getString());
                try
                {
                    Connection.Open();
                    string stringToInsert = @"INSERT INTO sessions (sessionCode, peopleInSession) VALUES (@SessionCode, 1)";

                    using (MySqlCommand command = new MySqlCommand(stringToInsert, Connection))
                    {
                        // Add parameters here
                        command.Parameters.AddWithValue("@SessionCode", CreateSessionCode);

                        command.Prepare();
                        int rowsUpdated = command.ExecuteNonQuery();

                        if (rowsUpdated > 0)
						{
                            ViewBag.SessionMessage = string.Format("Session code: {0}, people online: 1", CreateSessionCode);
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

            //JoinSessionCode has a value, Join a session
            else if (CreateSessionCode == "" && JoinSessionCode != "")
			{
                int peopleinsession;
                MySqlConnection Connection;
                Connection = new MySqlConnection(Connector.getString());

                try
                {
                    Connection.Open();
                    string stringToRead = @"SELECT DISTINCT sessionCode, peopleInSession FROM database.sessions WHERE sessionCode = @SessionCode";
                    MySqlCommand command = new MySqlCommand(stringToRead, Connection);

                    // Add parameters here
                    command.Parameters.AddWithValue("@SessionCode", JoinSessionCode);


                    using (MySqlDataReader myReader = command.ExecuteReader())
                    {
                        myReader.Read();
                        peopleinsession = Int32.Parse(myReader["peopleInSession"].ToString());
                    }

                    // updating the people online
                    string stringToInsert = @"UPDATE sessions SET peopleInSession = peopleInSession + 1 WHERE sessionCode = @SessionCode;";

                    using (MySqlCommand command2 = new MySqlCommand(stringToInsert, Connection))
                    {
                        // Add parameters here
                        command2.Parameters.AddWithValue("@SessionCode", JoinSessionCode);

                        command2.Prepare();
                        int rowsUpdated = command2.ExecuteNonQuery();

                        if (rowsUpdated > 0)
                        {
                            ViewBag.SessionMessage = string.Format("Session code: {0}, People online: {1}", JoinSessionCode, peopleinsession + 1);
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

            return View();
		}
	}
}