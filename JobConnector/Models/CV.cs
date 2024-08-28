using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobConnector.Models
{
    public class CV
    {
        public string email { get; set; }
        public string college_name { get; set; }
        public string college_degree { get; set; }
        public string college_period { get; set; }
        public string high_school_name { get; set; }
        public string high_school_subjects { get; set; }
        public string high_school_graduation { get; set; }
        public string company_name { get; set; }
        public string position { get; set; }
        public string company_period { get; set; }

        public CV(string email, string college_name, string college_degree, string college_period, string high_school_name, string high_school_subjects, string high_school_graduation, string company_name, string position, string company_period)
        {
            this.email = email;
            this.college_name = college_name;
            this.college_degree = college_degree;
            this.college_period = college_period;
            this.high_school_name = high_school_name;
            this.high_school_subjects = high_school_subjects;
            this.high_school_graduation = high_school_graduation;
            this.company_name = company_name;
            this.position = position;
            this.company_period = company_period;
        }
    }
}