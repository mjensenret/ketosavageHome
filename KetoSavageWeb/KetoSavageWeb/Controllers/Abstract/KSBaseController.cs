using KetoSavageWeb.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb.Controllers
{
    public abstract class KSBaseController : Controller
    {
        ApplicationUserManager _userManager;
        //ApplicationRoleManager _roleManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //public ApplicationRoleManager RoleManager
        //{
        //    get
        //    {
        //        return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
        //    }
        //    private set
        //    {
        //        _roleManager = value;
        //    }
        //}

        public KSBaseController()
        {

        }

        public KSBaseController(ApplicationUserManager userManager)
        {
            this._userManager = userManager;
        }
        //public static ApplicationUser GetCurrentUser(string userName, HttpSessionStateBase session, ApplicationUserManager userManager)
        //{
        //    // Make sure currently logged in user is the same user in the session
        //    if (session["currentUser"] == null || ((ApplicationUser)session["currentUser"]).UserName != userName)
        //    {
        //        session["currentUser"] = userManager.FindByNameAsync(userName);
        //    }

        //    return (ApplicationUser)session["currentUser"];
        //}
    }
}