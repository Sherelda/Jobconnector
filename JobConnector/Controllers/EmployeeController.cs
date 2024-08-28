using JobConnector.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace JobConnector.Controllers
{
    public class EmployeeController : Controller
    {
        public string connection_string = "Data Source=localhost\\SQLExpress;Database=jobconnector;Integrated Security=sspi;";

        // GET: Employee
        public ActionResult Index()
        {
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

        public ActionResult Resume()
        {
            var u = (User)Session["user"];

            SqlCommand command = null;
            SqlConnection conn = null;
            SqlDataReader reader = null;

            // read data from database
            try
            {
                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "select * from resumes where email=@email;";

                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("email", u.Email));

                reader = command.ExecuteReader();
                int records = 0;
                CV res = null;
                while (reader.Read())
                {
                    records++;
                    res = new CV(email: u.Email, college_name: reader[1].ToString(), college_degree: reader[2].ToString(), college_period: reader[3].ToString(), high_school_name: reader[4].ToString(), high_school_subjects: reader[5].ToString(), high_school_graduation: reader[6].ToString(), company_name: reader[7].ToString(), position: reader[8].ToString(), company_period: reader[9].ToString());

                }
                if (records > 0)
                {
                    ViewData["resume"] = res;
                }
                else
                {
                    ViewData["resume"] = null;
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

        [HttpPost]
        public ActionResult Resume(FormCollection f)
        {

            // add data to database
            SqlConnection conn = null;
            SqlCommand command = null;
            try
            {                
                var email = f["email"];
                var college_name = f["college_name"];
                var college_degree = f["college_degree"];
                var college_period = f["college_period"];
                var high_school_name = f["high_school_name"];
                var high_school_subjects = f["high_school_subjects"];
                var high_school_graduation = f["high_school_graduation"];
                var company_name = f["company_name"];
                var position = f["position"];
                var company_period = f["company_period"];

                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "update resumes set college_name=@cn, college_degree=@cd, college_period=@cp, high_school_name=@hsn, high_school_subjects=@hss, high_school_graduation=@hsg, company_name=@cn2, position=@p, company_period=@cp2 where email=@email";
                command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@cn", college_name);
                command.Parameters.AddWithValue("@cd", college_degree);
                command.Parameters.AddWithValue("@cp", college_period);
                command.Parameters.AddWithValue("@hsn", high_school_name);
                command.Parameters.AddWithValue("@hss", high_school_subjects);
                command.Parameters.AddWithValue("@hsg", high_school_graduation);
                command.Parameters.AddWithValue("@cn2", company_name);
                command.Parameters.AddWithValue("@p", position);
                command.Parameters.AddWithValue("@cp2", company_period);
                command.Parameters.AddWithValue("@email", email);
                int rowsAffected = command.ExecuteNonQuery();

                if(rowsAffected > 0)
                {
                    ViewData["success_message"] = "Resume Updated Successfully.";
                } else
                {
                    ViewData["warn_message"] = "No changes were made";
                }

                // -----------------------------
                sql = "select * from resumes where email=@email;";

                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("email", email));

                SqlDataReader reader = command.ExecuteReader();
                int records = 0;
                CV res = null;
                while (reader.Read())
                {
                    records++;
                    res = new CV(email: email, college_name: reader[1].ToString(), college_degree: reader[2].ToString(), college_period: reader[3].ToString(), high_school_name: reader[4].ToString(), high_school_subjects: reader[5].ToString(), high_school_graduation: reader[6].ToString(), company_name: reader[7].ToString(), position: reader[8].ToString(), company_period: reader[9].ToString());

                }
                if (records > 0)
                {
                    ViewData["resume"] = res;
                }
                else
                {
                    ViewData["resume"] = null;
                }

            }
            catch (Exception ex)
            {
                /*string qry = command.CommandText;
                foreach(SqlParameter p in command.Parameters)
                {
                    qry += qry.Replace(p.ParameterName, p.Value.ToString());
                }*/
                ViewData["error_message"] = "Error occurred: " + ex.Message;
                
            }
            finally
            {
                command.Dispose();
                conn.Close();
            }
            return View();
        }

        public ActionResult Documents()
        {
            // ---------------------
            SqlCommand command = null;
            SqlConnection conn = null;
            SqlDataReader reader = null;

            // read data from database
            try
            {
                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "select * from documents where email=@email";
                var u = (User)Session["user"];
                List<Doc> docs = new List<Doc>();

                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("email", u.Email));

                reader = command.ExecuteReader();
                int records = 0;
                while (reader.Read())
                {
                    records++;
                    docs.Add(new Doc(id: Int16.Parse(reader[0].ToString()), name: reader[2].ToString(), link: reader[3].ToString()));
                }
                if (records > 0)
                {
                    foreach(Doc d in docs)
                    {
                        if(d.Name.ToLower() == "id")
                        {
                            ViewData["id"] = d;
                        } else if (d.Name.ToLower() == "metric")
                        {
                            ViewData["metric"] = d;
                        } else if (d.Name.ToLower() == "degree")
                        {
                            ViewData["degree"] = d;
                        }
                    }
                }
                else
                {
                    ViewData["docs"] = null;
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
            // ---------------------

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
                var dt = DateTime.Now.ToString("MM-dd-yyyy-HH:mm:ss");
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
        public ActionResult Documents(HttpPostedFileBase id, HttpPostedFileBase metric, HttpPostedFileBase degree)
        {
            string msg = "";
            string err = "";
            // ---------
            try
            {
                int count = 0;
                // Handle file upload logic here
                if (id != null && id.ContentLength > 0)
                {
                    string result = SaveImage("ID", id);
                    if(result == "success")
                    {
                        msg += "ID uploaded     ";
                        count++;
                    } else
                    {
                        err += result;
                    }
                }

                if (metric != null && metric.ContentLength > 0)
                {
                    string result = SaveImage("Metric", metric);
                    if (result == "success")
                    {
                        msg += "Metric uploaded     ";
                        count++;
                    } else
                    {
                        err += result;
                    }
                }

                if (degree != null && degree.ContentLength > 0)
                {
                    string result = SaveImage("Degree", degree);
                    if (result == "success")
                    {
                        msg += "Degree uploaded";
                        count++;
                    } else
                    {
                        err += result;
                    }
                }

                if(count > 0)
                {
                    ViewBag.message = msg;
                } else
                {
                    ViewBag.message = "No documents were uploaded!!!";
                }

                if (err != "")
                {
                    ViewBag.error = err;
                }

            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
            }
            // -------

            

            ViewBag.message = msg;

            return View();
        }

        public ActionResult Find()
        {
            SqlCommand command = null;
            SqlConnection conn = null;
            SqlDataReader reader = null;

            // read data from database
            try
            {
                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "select * from jobs;";
                List<Job> jobs = new List<Job>();

                command = new SqlCommand(sql, conn);

                reader = command.ExecuteReader();
                int records = 0;
                while (reader.Read())
                {
                    records++;
                    jobs.Add(
                        new Job(
                            id: Int16.Parse(reader[0].ToString()), title: reader[1].ToString(), 
                            positions: Int16.Parse(reader[2].ToString()), description: reader[3].ToString(), 
                            qualifications: reader[4].ToString(), experience: reader[5].ToString(), 
                            specialization: reader[6].ToString(), lastDate: reader[7].ToString(), 
                            salary: double.Parse(reader[8].ToString()), jobType: reader[9].ToString(), 
                            companyName: reader[10].ToString(), companyEmail: reader[11].ToString(), 
                            companyAddress: reader[12].ToString(), companyWebsite: reader[13].ToString(),
                            companyLogo: reader[14].ToString(), addedBy: reader[15].ToString(), 
                            added: reader[16].ToString()));

                }
                if (records > 0)
                {
                    ViewData["jobs"] = jobs;
                }
                else
                {
                    ViewData["jobs"] = null;
                }
                reader.Close();

                string sql2 = "select * from payments where recipient=@recipient;";
                var u = (User)Session["user"];

                SqlCommand command2 = new SqlCommand(sql2, conn);
                command2.Parameters.Add(new SqlParameter("recipient", u.Email));

                SqlDataReader reader2 = command2.ExecuteReader();
                int records2 = 0;
                while (reader2.Read())
                {
                    records2++;
                    ViewData["record"] = reader2[1].ToString();
                }
                if(records2 > 0)
                {
                    ViewData["paid"] = true;
                }
                command2.Dispose();
                reader2.Close();
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
                    Session["job_id"] = id;
                }

                reader.Close();

                sql = "select job_id from applications where employee=@employee;";

                var u = (User)Session["user"];
                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("employee", u.Email));
                reader = command.ExecuteReader();
                int records2 = 0;
                var applied = new List<int>();
                while (reader.Read())
                {
                    records2++;
                    applied.Add(Int16.Parse(reader[0].ToString()));
                }

                if (records2 > 0)
                {
                    ViewData["applied"] = applied;
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

        [HttpPost]
        public ActionResult Job()
        {
            // add data to database
            SqlConnection conn = null;
            SqlCommand command = null;
            try
            {
                int id = Int16.Parse(Session["job_id"].ToString());
                var u = (User)Session["user"];

                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "insert into applications(employee, job_id) values(@employee, @job_id)";

                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("employee", u.Email));
                command.Parameters.Add(new SqlParameter("job_id", id));

                command.ExecuteNonQuery();
                

                ViewData["success_message"] = "Job Applied Successfully.";
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

        public ActionResult Terms()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Terms(string permission)
        {
            SqlConnection conn = null;
            SqlCommand command = null;


            try
            {
                var u = (User)Session["user"];
                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "update users set consent=@consent where email=@email;";

                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("email", u.Email));
                command.Parameters.Add(new SqlParameter("consent", "accepted"));

                command.ExecuteNonQuery();
                ViewData["success_message"] = "Details Successfully Updated.";

                u.Consent = "accepted";

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