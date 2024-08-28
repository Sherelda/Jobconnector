using JobConnector.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace JobConnector.Controllers
{

    public class EmployerController : Controller
    {
        public string connection_string = "Data Source=localhost\\SQLExpress;Database=jobconnector;Integrated Security=sspi;";
        public User user = null;

        public ActionResult Index()
        {
            if (Session["user"] != null)
            {
                user = Session["user"] as User;
            }

            return View();
        }

        public ActionResult Jobs()
        {

            List<Job> jobs = new List<Job>();
            SqlCommand command = null;
            SqlConnection conn = null;
            SqlDataReader reader;

            // read data from database
            try
            {
                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "select * from jobs where added_by=@added_by;";
                var u = (User)Session["user"];

                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("added_by", u.Email));

                reader = command.ExecuteReader();
                int records = 0;
                while (reader.Read())
                {
                    records++;
                    jobs.Add(new Job(id: Int16.Parse(reader[0].ToString()), title: reader[1].ToString(), positions: Int16.Parse(reader[2].ToString()), description: reader[3].ToString(),
                        qualifications: reader[4].ToString(), experience: reader[5].ToString(), specialization: reader[6].ToString(),
                        lastDate: reader[7].ToString(), salary: double.Parse(reader[8].ToString()), jobType: reader[9].ToString(),
                        companyName: reader[10].ToString(), companyEmail: reader[11].ToString(), companyAddress: reader[12].ToString(),
                        companyWebsite: reader[13].ToString(), companyLogo: reader[14].ToString(), addedBy: reader[15].ToString(),
                        added: reader[16].ToString()));

                }
                ViewData["jobs"] = jobs;

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

        [HttpGet]
        public ActionResult AddJob()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddJob(FormCollection form)
        {
            var job_title = form["job_title"];
            var positions = form["positions"];
            var description = form["description"];
            var qualifications = form["qualifications"];
            var experience = form["experience"];
            var specializations = form["specializations"];
            var last_date = form["last_date"];
            var salary = form["salary"];
            var job_type = form["job_type"];
            var company_name = form["company_name"];
            var company_email = form["company_email"];
            var company_website = form["company_website"];
            var company_address = form["company_address"];
            var company_logo = form["company_logo"];

            // add data to database
            SqlConnection conn = null;
            SqlCommand command = null;
            try
            {

                conn = new SqlConnection(connection_string);
                conn.Open();

                var u = (User)Session["user"];

                string sql = "insert into jobs(" +
                    "title, positions, description, qualifications,experience,specializations,last_date,salary,job_type,company_name,company_email,company_address,company_website,company_logo,added_by) " +
                    "values(@title, @positions, @description, @qualifications, @experience, @specializations, @last_date, @salary, @job_type, @company_name, @company_email, @company_address, @company_website, @company_logo, @added_by)";

                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("title", job_title));
                command.Parameters.Add(new SqlParameter("positions", positions));
                command.Parameters.Add(new SqlParameter("description", description));
                command.Parameters.Add(new SqlParameter("qualifications", qualifications));
                command.Parameters.Add(new SqlParameter("experience", experience));
                command.Parameters.Add(new SqlParameter("specializations", specializations));
                command.Parameters.Add(new SqlParameter("last_date", last_date));
                command.Parameters.Add(new SqlParameter("salary", salary));
                command.Parameters.Add(new SqlParameter("job_type", job_type));
                command.Parameters.Add(new SqlParameter("company_name", company_name));
                command.Parameters.Add(new SqlParameter("company_email", company_email));
                command.Parameters.Add(new SqlParameter("company_address", company_address));
                command.Parameters.Add(new SqlParameter("company_website", company_website));
                command.Parameters.Add(new SqlParameter("company_logo", company_logo));
                command.Parameters.Add(new SqlParameter("added_by", u.Email));

                command.ExecuteNonQuery();
                ViewData["success_message"] = "New Job Added Successfully.";
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

        [HttpGet]
        public ActionResult Job(int id)
        {

            SqlCommand command = null;
            SqlConnection conn = null;
            SqlDataReader reader = null;

            // read data from database
            try
            {
                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "select * from jobs where id=@id;";

                Job job = null;

                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("id", id));
                reader = command.ExecuteReader();
                int records = 0;
                while (reader.Read())
                {
                    records++;
                    job = new Job(
                            id: Int16.Parse(reader[0].ToString()), title: reader[1].ToString(),
                            positions: Int16.Parse(reader[2].ToString()), description: reader[3].ToString(),
                            qualifications: reader[4].ToString(), experience: reader[5].ToString(),
                            specialization: reader[6].ToString(), lastDate: reader[7].ToString(),
                            salary: double.Parse(reader[8].ToString()), jobType: reader[9].ToString(),
                            companyName: reader[10].ToString(), companyEmail: reader[11].ToString(),
                            companyAddress: reader[12].ToString(), companyWebsite: reader[13].ToString(),
                            companyLogo: reader[14].ToString(), addedBy: reader[15].ToString(),
                            added: reader[16].ToString());

                    ViewData["job"] = job;
                }

                reader.Close();


            }

            catch (Exception ex)
            {
                ViewData["error_message"] = "Error occurred: " + ex.Message;
            }
            finally
            {
                command?.Dispose();
                conn?.Close();
            }

            return View();
        }

        [HttpPost]
        public ActionResult Job(int id, string purpose)
        {
            // add data to database
            SqlConnection conn = null;
            SqlCommand command = null;
            try
            {

                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "delete from jobs where job_id=@id";

                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("job_id", id));

                command.ExecuteNonQuery();


                ViewData["success_message"] = "Job Deleted Successfully.";
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