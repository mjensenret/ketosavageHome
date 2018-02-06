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
    public class ManageUsersController : Controller
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

        public ManageUsersController()
        {

        }
        public ManageUsersController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        // GET: ManageUsers
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridViewPartial()
        {
            var users = from user in UserManager.Users
                        from role in RoleManager.Roles
                        where role.Users.Any(r => r.UserId == user.Id)
                        select new RegisterModel()
                        {
                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            Roles = role.Name,
                            SelectedRoleId = role.Id,
                            RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                            {
                                Selected = role.Name.Contains(x.Name),
                                Text = x.Name,
                                Value = x.Name
                            })
                        };
            var model = users.ToList();

            var systemRoles = RoleManager.Roles.ToList();

            //var items = (users
            //    .OrderBy(x => x.UserName)
            //    .Select(x => new
            //    {
            //        x.Id
            //        ,x.UserName
            //        ,x.FirstName
            //        ,x.LastName
            //        ,x.Email
            //        ,x.Roles
            //    })
            //    .ToList())
            //    .Select(x => new RegisterModel
            //    {
            //        UserName = x.UserName,
            //        FirstName = x.FirstName,
            //        LastName = x.LastName,
            //        Email = x.Email,
            //        Roles = string.Join(", ", systemRoles.Where(userRole => x.Roles.Select(r => r.RoleId).Contains(userRole.Id)).Select(r => r.Name).ToList())
            //    });

            return PartialView("_GridViewPartial", model);

        }


        public async Task<ActionResult> GridViewPartialAddNew(RegisterModel item)
        {
            if (ModelState.IsValid)
            {
                var newUser = new ApplicationUser
                {
                    UserName = item.UserName,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email

                };
                var result = await UserManager.CreateAsync(newUser, item.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(newUser.UserName, "Registered User");
                    return RedirectToAction("Index");
                }
                ViewBag.ErrorMessage = result.Errors.First();
            }
            else
            {
                ViewData["EditError"] = "Please correct all errors";
            }
            return PartialView("_GridViewPartial", UserManager.Users);
        }
    }
}