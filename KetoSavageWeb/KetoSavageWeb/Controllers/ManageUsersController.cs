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
        private ProgramRepository programRepository;

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
        public ManageUsersController(ApplicationUserManager userManager, ApplicationRoleManager roleManager, ProgramRepository pr)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            programRepository = pr;
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
                            Id = user.Id,
                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            Roles = role.Name,
                            SelectedRoleId = role.Name,
                            //RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                            //{
                            //    Selected = role.Name.Contains(x.Name),
                            //    Text = x.Name,
                            //    Value = x.Name
                            //})
                            
                        };
            var model = users.ToList();
            ViewBag.RoleId = new SelectList(RoleManager.Roles.ToList(), "Name", "Name");


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
                    await UserManager.AddToRoleAsync(newUser.Id, item.SelectedRoleId);
                    if (item.SelectedRoleId == "Client")
                    {
                        var clientProgram = new CoachedPrograms
                        {
                            ApplicationUser = newUser,
                            startDate = DateTime.Now
                        };
                        programRepository.Create(clientProgram);
                    }
                    return RedirectToAction("Index");
                }
                ViewBag.ErrorMessage = result.Errors.First();
            }
            else
            {
                ViewData["EditError"] = "Please correct all errors";
            }
            return RedirectToAction("GridViewPartial");
        }

        public async Task<ActionResult> GridViewPartialEdit(EditUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                var systemRoles = RoleManager.Roles.ToList();
                var userEdit = await UserManager.FindByNameAsync(model.UserName);

                if(userEdit == null)
                {
                    return HttpNotFound();
                }

                userEdit.FirstName = model.FirstName;
                userEdit.LastName = model.LastName;
                userEdit.Email = model.Email;

                var userRole = await UserManager.GetRolesAsync(userEdit.Id);

                if(userRole.Count() > 0)
                {
                    await UserManager.RemoveFromRolesAsync(userEdit.Id, userRole.ToArray());
                }

                var updRoleResult = await UserManager.AddToRoleAsync(userEdit.Id, model.SelectedRoleId);
                if(!updRoleResult.Succeeded)
                {
                    ModelState.AddModelError("", updRoleResult.Errors.First());
                }
                return RedirectToAction("Index");

            }
            ModelState.AddModelError("", "Something failed!");
            return View(model);
        }
    }
}