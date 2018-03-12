using KetoSavageWeb.Domain.Models;
using KetoSavageWeb.Domain.Repositories;
using KetoSavageWeb.Models;
using KetoSavageWeb.Util.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Controllers.Abstract
{
    public abstract class KSUserManagedController<TEntity, TRepository> : KSUserManagedController<TEntity, TRepository, TEntity>
        where TEntity : class, IUserManaged, IKeyedEntity<int>, IHasIsNew, new()
        where TRepository : UserManagedRepository<TEntity>
    {
        protected KSUserManagedController(TRepository r) : base(r) { }
        protected KSUserManagedController(TRepository r, Func<TEntity, string> userKeyField) : base(r, userKeyField) { }

        protected override TEntity createViewModel(TEntity entity)
        {
            return entity;
        }
    }

    public abstract class KSUserManagedController<TEntity, TRepository, TViewModel> : UserManagedController<TEntity, TRepository, TViewModel>
    where TEntity : class, IUserManaged, IKeyedEntity<int>, new()
    where TViewModel : IKeyedEntity<int>, IHasIsNew
    where TRepository : UserManagedRepository<TEntity>
    {
        private ApplicationUserManager _userManager = null;
        private ApplicationUser _currentUser = null;

        protected KSUserManagedController(TRepository r) : base(r) { }
        protected KSUserManagedController(TRepository r, Func<TEntity, string> userKeyField) : base(r, userKeyField) { }

        public ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                    _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                return _userManager;
            }
            private set { _userManager = value; }
        }

        public ApplicationUser CurrentUser
        {
            get
            {
                if (_currentUser == null && User.Identity.IsAuthenticated)
                {
                    _currentUser = KSBaseController.GetCurrentUser(Convert.ToInt32(User.Identity.GetUserId()), Session, UserManager);
                }
                return _currentUser;
            }
        }

    }

}