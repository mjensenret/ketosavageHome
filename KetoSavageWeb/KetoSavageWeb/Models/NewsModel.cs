using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class NewsModel
    {
        public int Id { get; set; }
        public string Headline { get; set; }
        public string Author { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }

        [DataType(DataType.Date)]
        public DateTime Expires { get; set; }
    }
}