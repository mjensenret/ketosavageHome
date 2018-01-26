using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories.Abstract;
using KetoSavageWeb.Util.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Repositories
{
    public class UserProfileRepository : GenericRepository<UserProfile>
    {
        private IDataLayer<UserRole> userRoleLayer;

        public UserProfileRepository(IDataLayer<UserProfile> dataLayer, IDataLayer<UserRole> userRoleLayer)
            : base(dataLayer)
        {
            this.userRoleLayer = userRoleLayer;
        }

        public IQueryable<UserRole> GetUserRoles
        {
            get { return this.userRoleLayer.Get; }
        }
    }
}