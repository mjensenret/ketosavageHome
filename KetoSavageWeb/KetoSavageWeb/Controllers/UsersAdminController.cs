using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        ApplicationRoleManager _roleManager;
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

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public UsersAdminController()
        {

        }
        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }


        // GET: UsersAdmin

        public async Task<ActionResult> Index(ListModel<UserListViewModel> model)
        {
            var userQuery = UserManager.Users;

            var items = (await userQuery
                .OrderBy(x => x.UserName)
                //.Page(model.Page, model.ItemsPerPage, model.TotalItems)
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    x.Email
                })
                .ToListAsync())
                .Select(x => new UserListViewModel
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email
                });

            model.Items = items;

            return View(model);
        }

        public ActionResult userGridPartialView(ListModel<UserListViewModel> model)
        {
            var userQuery = UserManager.Users;

            var items = (userQuery
                .OrderBy(x => x.UserName)
                //.Page(model.Page, model.ItemsPerPage, model.TotalItems)
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    x.Email
                })
                .ToList())
                .Select(x => new UserListViewModel
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email
                });

            model.Items = items;


            return PartialView("UserGridPartialView", model);
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

    //public async Task<ActionResult> CreateRole(RoleViewModel roleViewModel)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        // Initialize ApplicationRole
    //        var role = new ApplicationRole(roleViewModel.Name);
    //        var roleResult = await ApplicationRoleManager.CreateAsync(role);
    //    }
    //}
}