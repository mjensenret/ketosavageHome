using KetoSavageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace KetoSavageWeb.Controllers.Abstract
{
    public abstract class KSBaseAPIController : ApiController
    {
        private ApplicationUser _user;
        private ApplicationUserManager _userManager;

        public KSBaseAPIController()
        {

        }

        public KSBaseAPIController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                    _userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                return _userManager;
            }
            private set
            {
                _userManager = value;
            }
        }
    }
}
