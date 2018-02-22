using KetoSavageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb.Controllers
{
    public abstract class KSBaseController : Controller
    {
        public static ApplicationUser GetCurrentUser(string userName, HttpSessionStateBase session, ApplicationUserManager userManager)
        {
            // Make sure currently logged in user is the same user in the session
            if (session["currentUser"] == null || ((ApplicationUser)session["currentUser"]).UserName != userName)
            {
                session["currentUser"] = userManager.FindByNameAsync(userName);
            }

            return (ApplicationUser)session["currentUser"];
        }
    }
}