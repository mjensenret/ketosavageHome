using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb.Controllers
{
    [Authorize(Roles = "Admin, Coach")]
    public class UsersAdminController : Controller
    {
        private UserProfileRepository userRepository;

        ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }
                return _userManager;
            }
        }

        public UsersAdminController(UserProfileRepository r)
        {
            this.userRepository = r;
        }

        // GET: UsersAdmin

        public ActionResult Index()
        {
            var q = this.userRepository.GetUsers();

            var model = q
                .Select(u => new EditUserViewModel
                {
                    Id = u.Id
                    ,
                    FirstName = u.FirstName
                    ,
                    LastName = u.LastName
                    ,
                    Email = u.Email
                    ,
                    UserName = u.UserName
                })
                .OrderBy(u => u.LastName);

            return View(model);
        }

        public async Task<ActionResult> AddUser(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                var user = new ApplicationUser
                {
                    UserName = model.UserName
                    , Email = model.Email
                    , FirstName = model.FirstName
                    , LastName = model.LastName
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    
                }
                ViewBag.ErrorMessage = result.Errors.First();
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}