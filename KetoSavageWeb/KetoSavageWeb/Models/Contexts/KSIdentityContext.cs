using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models.Contexts
{
    public class KSIdentityContext : IdentityDbContext<ApplicationUser>
    {
        public KSIdentityContext()
            : base("KSDataConnection", throwIfV1Schema: false)
        {
        }

        static KSIdentityContext()
        {
            //Database.SetInitializer<KSIdentityContext>(new ApplicationDbInitializer());
        }

        public static KSIdentityContext Create()
        {
            return new KSIdentityContext();
        }
    }
}