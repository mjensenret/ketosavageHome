using KetoSavageWeb.Models;
using Microsoft.AspNet.Identity;
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
        public static ApplicationUser GetCurrentUser(int userId, HttpSessionStateBase session, ApplicationUserManager userManager)
        {
            // Make sure currently logged in user is the same user in the session
            if (session["currentUser"] == null || ((ApplicationUser)session["currentUser"]).Id != userId)
            {
                session["currentUser"] = userManager.FindById(userId);
            }

            return (ApplicationUser)session["currentUser"];
        }


        private ApplicationUserManager _userManager = null;
        private ApplicationUser _currentUser = null;
        //private KSDataContext

        public KSBaseController()
        {
        }

        public KSBaseController(ApplicationUserManager userManager)
        {
            this._userManager = userManager;
        }

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

        public ApplicationUser CurrentUser
        {
            get
            {
                if (_currentUser == null && User.Identity.IsAuthenticated)
                {
                    _currentUser = GetCurrentUser(Convert.ToInt32(User.Identity.GetUserId()), Session, UserManager);
                }
                return _currentUser;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }
            base.Dispose(disposing);
        }



    }
}