using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobConnector.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Positions { get; set; }
        public string Description { get; set; }
        public string Qualifications { get; set; }
        public string Experience { get; set; }
        public string Specialization { get; set; }
        public string LastDate { get; set; }
        public double Salary { get; set; }
        public string JobType { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyLogo { get; set; }
        public string AddedBy { get; set; }
        public string Added { get; set; }

        public Job(int id, string title, int positions, string description, string qualifications, string experience, string specialization, string lastDate, double salary, string jobType, string companyName, string companyEmail, string companyAddress, string companyWebsite, string companyLogo, string addedBy, string added)
        {
            Id = id;
            Title = title;
            Positions = positions;
            Description = description;
            Qualifications = qualifications;
            Experience = experience;
            Specialization = specialization;
            LastDate = lastDate;
            Salary = salary;
            JobType = jobType;
            CompanyName = companyName;
            CompanyEmail = companyEmail;
            CompanyAddress = companyAddress;
            CompanyWebsite = companyWebsite;
            CompanyLogo = companyLogo;
            AddedBy = addedBy;
            Added = added;

        }
    }
}