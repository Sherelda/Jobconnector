using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobConnector.Models
{
    public class Doc
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }

        public Doc(int id, string name, string link)
        {
            this.Id = id;
            this.Name = name;
            this.Link = link;
        }
    }
}