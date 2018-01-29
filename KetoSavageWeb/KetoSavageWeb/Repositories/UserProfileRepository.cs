using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using KetoSavageWeb.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using KetoSavageWeb.Repositories;
using System.Web;

namespace KetoSavageWeb.Repositories
{
    public class UserProfileRepository
    {
        //private KSDataContext _context { get { return HttpContext.GetOwinContext().Get<ApplicationUserManager>(); } }

        //private IDataLayer<UserRole> userRoleLayer;

        //public UserProfileRepository(IDataLayer<ApplicationUser> dataLayer, IDataLayer<UserRole> userRoleLayer)
        //    : base(dataLayer)
        //{
        //    this.userRoleLayer = userRoleLayer;
        //}

        //public IQueryable<UserRole> GetUserRoles
        //{
        //    get { return this.userRoleLayer.Get; }
        //}

        //public IEnumerable<ApplicationUser> GetUsers()
        //{
        //    return db.Users;
        //}
    }
}