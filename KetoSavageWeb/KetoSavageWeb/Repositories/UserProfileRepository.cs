using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories.Abstract;
using KetoSavageWeb.Util.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Repositories
{
    public class UserProfileRepository //: GenericRepository<ApplicationUser>
    {
        private KSDataContext db = new KSDataContext();
        
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

        public IEnumerable<ApplicationUser> GetUsers()
        {
            return db.Users;
        }
    }
}