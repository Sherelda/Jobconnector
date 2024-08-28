using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobConnector.Models
{
    public class User
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string UserType { get; set; }
        public string Resume { get; set; }
        public string Consent { get; set; }

        public User(string email, string name, string password, string userName, string address, string country, string phone, string userType, string res = "", string cons = "")
        {
            Email = email;
            Name = name;
            Password = password;
            UserName = userName;
            Address = address;
            Country = country;
            Phone = phone;
            UserType = userType;
            Resume = res;
            Consent = cons;
        }
    }
}