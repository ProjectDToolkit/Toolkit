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
        private readonly IWebHostEnvironment environment;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(FileUpload obj)
        {
            try
            {
                string strDateTime = System.DateTime.Now.ToString("ddMMyyyyHHMMss");
                string finalPath = "\\wwwroot\\Files\\" + strDateTime + obj.UploadFile.fileName;

                obj.UploadFile.SaveAs(Directory.GetCurrentDirectory() + ("~") + finalPath);
                obj.FilePath = finalPath;
                ViewBag.Message = SaveToDB(obj);
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
                HttpContext.Session.SetString("idSession", obj.IdSession);
                HttpContext.Session.SetString("filename", obj.FileName);
                HttpContext.Session.SetString("fileDesc", obj.FileDesc);
                HttpContext.Session.SetString("filePath", obj.FilePath);
                if (HttpContext.Session.GetString("SessionCode") != null)
                {
                    string idSession = HttpContext.Session.GetString("idSession");
                    string fileName = HttpContext.Session.GetString("fileName");
                    string fileDesc = HttpContext.Session.GetString("fileDesc");
                    string filePath = HttpContext.Session.GetString("filePath");

                    MySqlConnection Connection;
                    Connection = new MySqlConnection(Connector.getString());
                    Connection.Open();
                
                        string stringToInsert = @"INSERT INTO file (idSession, filName, filePath,   onDate) VALUES (@idSession, @filName, @filePath, GETDATE())";

                    using (MySqlCommand command = new MySqlCommand(stringToInsert, Connection))
                    {
                        command.Parameters.AddWithValue("@idSession", idSession);
                        command.Parameters.AddWithValue("@filName", fileName);
                        command.Parameters.AddWithValue("@filePath", filePath);

                        command.Prepare();
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



        /*
        [HttpPost("FileUpload")]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string test = AppContext.BaseDirectory;
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\Files", formFile.FileName);
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            return View();
        }
        */

