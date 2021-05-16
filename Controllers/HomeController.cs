using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MySql.Data.MySqlClient;
using ProjectD.Database;
using ProjectD.Models;

namespace ProjectD.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index(string message = "")
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public IActionResult JoinSession(HomeModel hm)
		{
            MySqlConnection Connection;
            Connection = new MySqlConnection(Connector.getString());

            

            try
            {
                Connection.Open();
                string stringToRead = @"SELECT DISTINCT sessionCode FROM database.sessions where sessionCode = @SessionCode";
                MySqlCommand command = new MySqlCommand(stringToRead, Connection);

                // Add parameters here
                command.Parameters.AddWithValue("@SessionCode", hm.JoinSessionCode);

                MySqlDataReader reader = command.ExecuteReader();
                if (!reader.HasRows)
				{
                    return RedirectToAction("Index", "Home", new { message = "Session Code not found" });
                }
                else
				{
                    return RedirectToAction("Index", "SessionStart", new { JoinSessionCode = hm.JoinSessionCode });
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

		[HttpPost]
        public IActionResult CreateSession(HomeModel hm)
        {
            return RedirectToAction("Index", "SessionStart", new { CreateSessionCode = hm.CreateSessionCode });
        }
    }
}