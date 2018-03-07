using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class Role : IdentityRole<int, UserRole>
    {
        public Role() : base()
        {

        }
    }
}