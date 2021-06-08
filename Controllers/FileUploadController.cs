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
                string sessionId = HttpContext.Session.GetString("SessionCode");
                string strDateTime = System.DateTime.Now.ToString("ddMMyyyyHHMMss");
                string finalPath = "\\wwwroot\\Files\\" +sessionId + "\\" + obj.UploadFile.FileName;
                obj.FilePath = finalPath;
                ViewBag.Message = SaveToDB(obj);

                var folderpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files\\" + sessionId + "\\");
                if (!Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
                }

                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files\\" + sessionId + "\\", obj.UploadFile.FileName);
                using (var stream = System.IO.File.Create(filepath))
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

                    
                    var splitFilePath = filePath.Split(new string[] { idSession }, StringSplitOptions.None);
                    idSession = idSession.ToUpper();
                    filePath = splitFilePath[0] + idSession + splitFilePath[1];
                        
                    MySqlConnection Connection;
                    Connection = new MySqlConnection(Connector.getString());
                    Connection.Open();

                    string stringToInsert = $"SELECT * from database.files WHERE idSession = '{idSession}' AND fileName = '{fileName}' AND fileDesc = '{fileDesc}';";
                    MySqlCommand cmd = new MySqlCommand(stringToInsert, Connection);
                    MySqlDataReader reader;
                    reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        MySqlConnection Connection2;
                        Connection2 = new MySqlConnection(Connector.getString());
                        Connection2.Open();
                        stringToInsert = @"INSERT INTO files (idSession, fileName, fileDesc, filePath) VALUES (@idSession, @fileName, @fileDesc, @filePath)";
                        using (MySqlCommand command = new MySqlCommand(stringToInsert, Connection2))
                        {
                            command.Parameters.AddWithValue("@idSession", idSession);
                            command.Parameters.AddWithValue("@fileName", fileName);
                            command.Parameters.AddWithValue("@fileDesc", fileDesc);
                            command.Parameters.AddWithValue("@filePath", filePath);

                            command.Prepare();
                            command.ExecuteNonQuery();
                            
                        }
                        Connection2.Close();
                        Connection.Close();
                        return "Saved Successfully";
                    }
                    else
                    {
                        Connection.Close();
                        return string.Format("There is already a file with this name and description!");
                    }
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

        public IActionResult FileList()
        {
            List<FileModel> fileList = new List<FileModel>();

            MySqlConnection connection;
            connection = new MySqlConnection(Connector.getString());

            try
            {
                connection.Open();

                string query = $"SELECT * FROM database.files WHERE idSession = '{HttpContext.Session.GetString("SessionCode")}';";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var file = new FileModel
                    {
                        idFiles = reader.GetInt32("idFiles"),
                        idSession = reader.GetString("idSession"),
                        fileName = reader.GetString("fileName"),
                        fileDesc = reader.GetString("fileDesc"),
                        filePath = reader.GetString("filePath")
                    };
                    fileList.Add(file);
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
            return View(fileList);
        }

        public FileResult Download(string filePath)
        {
            string sessionId = HttpContext.Session.GetString("SessionCode");
            sessionId = sessionId.ToUpper();
            var splitFilePath = filePath.Split(new string[] { sessionId }, StringSplitOptions.None);
            string fileName = splitFilePath[1];
            string directory = Directory.GetCurrentDirectory();
            string fullPath = directory + filePath;
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public IActionResult DeleteFile (int idFile)
        {
            MySqlConnection Connection;
            Connection = new MySqlConnection(Connector.getString());

            try
            {
                Connection.Open();
                string query = @"DELETE FROM database.files WHERE idfiles = @qid;";
                MySqlCommand cmd = new MySqlCommand(query, Connection);
                cmd.Parameters.AddWithValue("@qid", idFile);
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
            return RedirectToAction("FileList", "FileUpload");
        }
    }
        
}
