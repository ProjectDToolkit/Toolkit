using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ProjectD.Models;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using Microsoft.Extensions.Hosting.Internal;
using ProjectD.Database;


namespace ProjectD.Controllers
{
    public class FileUploadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(FileUpload obj)
        {
            try
            {
                string strDateTime = System.DateTime.Now.ToString("ddMMyyyyHHMMss");
                string finalPath = "\\wwwroot\\Files\\" + strDateTime + obj.UploadFile.FileName;
                obj.FilePath = finalPath;
                ViewBag.Message = SaveToDB(obj);


                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", obj.UploadFile.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await obj.UploadFile.CopyToAsync(stream);
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                return View();
            }
        }
        public string SaveToDB(FileUpload obj)
        {
            try
            {
                HttpContext.Session.SetString("fileName", obj.FileName);
                HttpContext.Session.SetString("fileDesc", obj.FileDesc);
                HttpContext.Session.SetString("filePath", obj.FilePath);
                if (HttpContext.Session.GetString("SessionCode") != null)
                {
                    string idSession = HttpContext.Session.GetString("SessionCode");
                    string fileName = HttpContext.Session.GetString("fileName");
                    string fileDesc = HttpContext.Session.GetString("fileDesc");
                    string filePath = HttpContext.Session.GetString("filePath");

                    MySqlConnection Connection;
                    Connection = new MySqlConnection(Connector.getString());
                    Connection.Open();

                    string stringToInsert = @"INSERT INTO files (idSession, fileName, filePath) VALUES (@idSession, @fileName, @filePath)";

                    using (MySqlCommand command = new MySqlCommand(stringToInsert, Connection))
                    {
                        command.Parameters.AddWithValue("@idSession", idSession);
                        command.Parameters.AddWithValue("@fileName", fileName);
                        command.Parameters.AddWithValue("@filePath", filePath);

                        command.Prepare();
                        command.ExecuteNonQuery();
                    }
                    return "Saved Successfully";
                }
                else
                {
                    return string.Format("Error Something went wrong");
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
    }
}
