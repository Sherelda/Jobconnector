using JobConnector.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace JobConnector.Controllers
{
    public class AdminController : Controller
    {
        public string connection_string = "Data Source=localhost\\SQLExpress;Database=jobconnector;Integrated Security=sspi;";

        // GET: Admin
        public ActionResult Index()
        {
            SqlCommand command = null;
            SqlConnection conn = null;
            SqlDataReader reader;

            // read data from database
            try
            {
                int users = 0;
                int jobs = 0;
                int cvs = 0;
                int contacted = 0;
                int partnercvs = 0;
                conn = new SqlConnection(connection_string);
                conn.Open();

                // count jobs
                string sql = "select count(*) from jobs;";
                command = new SqlCommand(sql, conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    jobs = Int16.Parse(reader[0].ToString());
                }
                reader.Close();

                // count users
                sql = "select count(*) from users;";
                command = new SqlCommand(sql, conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    users = Int16.Parse(reader[0].ToString());
                }
                reader.Close();

                // count cvs
                sql = "select count(*) from resumes where college_name!=@name;";
                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("name", ""));
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    cvs = Int16.Parse(reader[0].ToString());
                }
                reader.Close();

                // count partner cvs
                sql = "select count(*) from documents where email in(select email from users where user_type=@partner);";
                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("partner", "partner"));
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    partnercvs = Int16.Parse(reader[0].ToString());
                }
                reader.Close();

                // count contacted
                sql = "select count(*) from contacted";
                command = new SqlCommand(sql, conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    contacted = Int16.Parse(reader[0].ToString());
                }
                reader.Close();

                ViewData["users"] = users;
                ViewData["cvs"] = cvs;
                ViewData["contacted"] = contacted;
                ViewData["jobs"] = jobs;
                ViewData["partnercvs"] = partnercvs;

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

                string sql = "select * from jobs;";
                var u = (User)Session["user"];

                command = new SqlCommand(sql, conn);

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

        public User GetUser(string email)
        {
            SqlCommand command = null;
            SqlConnection conn = null;
            SqlDataReader reader;
            User user = null;

            // read data from database
            try
            {
                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "select * from users where email=@email;";
                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("email", email));
                reader = command.ExecuteReader();
                int records = 0;
                while (reader.Read())
                {
                    records++;
                    user = new User(email: reader[0].ToString(), name: reader[1].ToString(), password: reader[5].ToString(), userName: reader[4].ToString(), address: reader[3].ToString(), country: reader[7].ToString(), phone: reader[2].ToString(), userType: reader[6].ToString(), res: reader[8].ToString());

                }
            }
            catch (Exception )
            {
                user = null;
            }
            finally
            {
                command.Dispose();
                conn.Close();
            }

            return user;
        }

        public List<User> GetUsers(string user_type)
        {
            var users = new List<User>();
            SqlCommand command = null;
            SqlConnection conn = null;
            SqlDataReader reader;

            // read data from database
            try
            {
                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "select * from users where user_type=@user_type;";
                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("user_type", user_type));
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new User(email: reader[0].ToString(), name: reader[1].ToString(), password: reader[5].ToString(), userName: reader[4].ToString(), address: reader[3].ToString(), country: reader[7].ToString(), phone: reader[2].ToString(), userType: reader[6].ToString(), res: reader[8].ToString()));

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

            return users;
        }

        public ActionResult Partners()
        {
            var partners = GetUsers("partner");
            ViewData["users"] = partners;
            return View();
        }

        public ActionResult Employees()
        {
            var employees = GetUsers("employee");
            ViewData["users"] = employees;
            return View();
        }

        public ActionResult Resumes()
        {
            SqlCommand command = null;
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<Record> records = new List<Record>();

            // read data from database
            try
            {
                conn = new SqlConnection(connection_string);
                conn.Open();

                string sql = "select * from resumes";

                command = new SqlCommand(sql, conn);
                //command.Parameters.Add(new SqlParameter("email", u.Email));

                reader = command.ExecuteReader();
                while (reader.Read())
                {

                    var user = GetUser(reader[0].ToString());
                    var cv = new CV(email: reader[0].ToString(), college_name: reader[1].ToString(), college_degree: reader[2].ToString(), college_period: reader[3].ToString(), high_school_name: reader[4].ToString(), high_school_subjects: reader[5].ToString(), high_school_graduation: reader[6].ToString(), company_name: reader[7].ToString(), position: reader[8].ToString(), company_period: reader[9].ToString());
                    if(cv.college_name != "") {
                        records.Add(new Record(user, cv));
                    }                    
                }
                ViewData["records"] = records;
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

        public ActionResult Employers()
        {
            var employers = GetUsers("employer");
            ViewData["users"] = employers;
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

        public ActionResult CVs()
        {
            
            SqlCommand command = null;
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<Doc> records = new List<Doc>();

            // read data from database
            try
            {
                conn = new SqlConnection(connection_string);
                conn.Open();
                string sql = "select * from documents where email in(select email from users where user_type='partner');";
                command = new SqlCommand(sql, conn);
                //command.Parameters.Add(new SqlParameter("email", u.Email));

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    records.Add(new Doc(Int16.Parse(reader[0].ToString()), name: reader[2].ToString(), link: reader[3].ToString()));
                    
                }
                ViewData["cvs"] = records;
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

    public class Record
    {
        public User u { get; set; }
        public CV cv { get; set; }

        public Record(User u, CV cv)
        {
            this.u = u;
            this.cv = cv;
        }
    }
}