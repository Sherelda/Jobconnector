using JobConnector.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobConnector.Controllers
{
    public class PartnerController : Controller
    {
        public string connection_string = "Data Source=localhost\\SQLExpress;Database=jobconnector;Integrated Security=sspi;";

        // GET: Partner
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddCVs()
        {
            return View();
        }

        public string SaveImage(string name, HttpPostedFileBase file)
        {
            // add data to database
            SqlConnection conn = null;
            SqlCommand command = null;
            try
            {
                // Save the file to the server
                var u = (User)Session["user"];
                var dt = DateTime.Now.ToString("dd_MMMM_yyyy_HH:mm:ss");
                var fileName = u.Email + "_" + name + ".pdf";
                var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
                file.SaveAs(path);

                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "insert into documents(email, doc_name, doc_link) values(@email, @doc_name, @doc_link);";

                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("email", u.Email));
                command.Parameters.Add(new SqlParameter("doc_name", name));
                command.Parameters.Add(new SqlParameter("doc_link", fileName));
                command.ExecuteNonQuery();

                command.Dispose();
                conn.Close();
                return "success";
            }
            catch (Exception ex)
            {
                command?.Dispose();
                conn?.Close();

                return ex.Message;
            }
        }



        [HttpPost]
        public ActionResult AddCVs(List<HttpPostedFileBase> files)
        {
            try
            {
                var successes = new List<string>();
                var fails = new List<string>();
                var f = "";
                foreach (var file in files)
                {
                    // Handle file upload logic here
                    if (file != null && file.ContentLength > 0)
                    {

                        string result = SaveImage(file.FileName, file);
                        if (result == "success")
                        {
                            successes.Add(file.FileName);
                        }
                        else
                        {
                            fails.Add(file.FileName);
                            f += result + "\n";
                        }
                    }
                }
                ViewData["successes"] = successes;
                ViewData["error_message"] = f;
                ViewData["fails"] = fails;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }


        [HttpGet]
        public ActionResult MyProfile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MyProfile(FormCollection collection)
        {

            // update data in database
            // add data to database
            SqlConnection conn = null;
            SqlCommand command = null;


            try
            {
                var fullname = collection["fullname"];
                var phone = collection["phone"];
                var password = collection["password"];
                var address = collection["address"];
                var email = collection["email"];

                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "update users set fullname=@fullname, phone=@phone, address=@address, password=@password where email=@email;";

                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("fullname", fullname));
                command.Parameters.Add(new SqlParameter("phone", phone));
                command.Parameters.Add(new SqlParameter("address", address));
                command.Parameters.Add(new SqlParameter("email", email));
                command.Parameters.Add(new SqlParameter("password", password));

                command.ExecuteNonQuery();
                ViewData["success_message"] = "Details Successfully Updated.";
                var u = (User)Session["user"];

                u.Name = fullname;
                u.Phone = phone;
                u.Password = password;
                u.Address = address;

                Session["user"] = u;
            }
            catch (Exception ex)
            {
                ViewData["error_message"] = "Error occurred: " + ex.Message;
            }
            finally
            {
                command.Dispose();
                conn.Close();
            }

            return View();
        }

    }
}