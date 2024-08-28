using JobConnector.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace JobConnector.Controllers
{
    public class HomeController : Controller
    {
        public string connection_string = "Data Source=localhost\\SQLExpress;Database=jobconnector;Integrated Security=sspi;";
        public User user = null;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {
            var username = collection["username"];
            var fullname = collection["name"];
            var email = collection["email"];
            var phone = collection["phone"];
            var address = collection["address"];
            var country = collection["country"];
            var user_type = collection["user_type"];
            var password = collection["password"];

            // add data to database
            SqlConnection conn = null;
            SqlCommand command = null;
            try
            {
                
                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "insert into users(" +
                    "email, fullname, phone, address, username, password, user_type, country, cv) " +
                    "values(@email, @fullname, @phone, @address, @username, @password, @user_type, @country, @cv)";

                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("email", email));
                command.Parameters.Add(new SqlParameter("fullname", fullname));
                command.Parameters.Add(new SqlParameter("phone", phone));
                command.Parameters.Add(new SqlParameter("address", address));
                command.Parameters.Add(new SqlParameter("username", username));
                command.Parameters.Add(new SqlParameter("password", password));
                command.Parameters.Add(new SqlParameter("user_type", user_type));
                command.Parameters.Add(new SqlParameter("country", country));
                command.Parameters.Add(new SqlParameter("cv", ""));

                command.ExecuteNonQuery();
                sql = "insert into resumes values(@email, @cn, @cd, @cp, @hsn, @hss, @hsg, @cn2, @p, @cp2)";
                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("email", email));
                command.Parameters.Add(new SqlParameter("cn", ""));
                command.Parameters.Add(new SqlParameter("cd", ""));
                command.Parameters.Add(new SqlParameter("cp", ""));
                command.Parameters.Add(new SqlParameter("hsn", ""));
                command.Parameters.Add(new SqlParameter("hss", ""));
                command.Parameters.Add(new SqlParameter("hsg", ""));
                command.Parameters.Add(new SqlParameter("cn2", ""));
                command.Parameters.Add(new SqlParameter("p", ""));
                command.Parameters.Add(new SqlParameter("cp2", ""));
                command.ExecuteNonQuery();

                ViewData["success_message"] = "New Account Created Successfully.";
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

        public ActionResult Login()
        {
            if(Session["user"] != null)
            {
                user = (User)Session["user"];
                string user_type = user.UserType;
                TempData["user"] = user;
                if (user_type == "employee")
                {
                    return RedirectToAction("MyProfile");
                }
                else if (user_type == "employer")
                {
                    return RedirectToAction("Index", "Employer");
                }
                else if (user_type == "partner")
                {
                    return RedirectToAction("Index", "Partner");
                }
                else if (user_type == "admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var email = collection["email"];
            var user_type = collection["user_type"];
            var password = collection["password"];

            SqlCommand command = null;
            SqlConnection conn = null;
            SqlDataReader reader = null;

            // read data from database
            try
            {
                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "select * from users where email=@email and password=@password and user_type=@user_type;";

                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("email", email));
                command.Parameters.Add(new SqlParameter("password", password));
                command.Parameters.Add(new SqlParameter("user_type", user_type));

                reader = command.ExecuteReader();
                int records = 0;
                while (reader.Read())
                {
                    records++;
                    user = new User(email: email, name: reader[1].ToString(), password: password, userName: reader[4].ToString(), address: reader[3].ToString(), country: reader[7].ToString(), phone: reader[2].ToString(), userType: reader[6].ToString(), res: reader[8].ToString(), cons: reader[9].ToString());
                    
                }
                if(records > 0)
                {
                    Session["user"] = user;
                    ViewData["success_message"] = "Successfully logged in.";
                    if (user_type == "employee")
                    {
                        return RedirectToAction("MyProfile", "Employee");
                    } else if(user_type == "employer")
                    {
                        return RedirectToAction("Index", "Employer");
                    } else if (user_type == "partner") 
                    {
                        return RedirectToAction("Index", "Partner");
                    }
                    else if (user_type == "admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                } else
                {
                    Session["user"] = null;
                    ViewData["error_message"] = "Invalid email, password or user type";
                }
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
                
        public ActionResult Logout()
        {

            Session["user"] = null;
            return RedirectToAction("Login");
        }

        public ActionResult Success(string purpose, string email)
        {


            // -----------------------------------------------------
            // add data to database
            SqlConnection conn = null;
            SqlCommand command = null;
            try
            {
                string p = "";
                if (purpose == "search")
                {
                    ViewData["msg"] = "A Search Filter";
                    p = "search/filter";
                }
                else if (purpose == "cv5")
                {
                    ViewData["msg"] = "5 CVs";
                    p = "5 CVs";
                }
                else if (purpose == "cv10")
                {
                    ViewData["msg"] = "10 CVs";
                    p = "10 CVs";
                }

                //ViewData["message"] = p;

                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "insert into payments(recipient, purpose) values(@user, @p)";
                command = new SqlCommand(sql, conn);
                var u = (User)Session["user"];
                command.Parameters.Add(new SqlParameter("user", u.Email));
                command.Parameters.Add(new SqlParameter("p", p));
                command.ExecuteNonQuery();

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
            // ----------------------------------------------------
            var r = Request.Params;
            return View();
        }
    }
}