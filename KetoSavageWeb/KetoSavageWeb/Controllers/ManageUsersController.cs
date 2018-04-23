using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace KetoSavageWeb.Controllers
{
    public class ManageUsersController : KSBaseController
    {

        private ProgramRepository programRepository;
        private RoleRepository roleRepository;

        ApplicationUserManager _userManager;

        public ManageUsersController(ProgramRepository pr, RoleRepository rr)
        {
            programRepository = pr;
            roleRepository = rr;
        }

        // GET: ManageUsers
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridViewPartial()
        {

            var userQuery = UserManager.Users.Where(x => x.IsActive == true).Include(r => r.Roles);
            var items = (userQuery
                .OrderBy(x => x.UserName)
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    x.FirstName,
                    x.LastName,
                    x.Email,
                    Roles = x.Roles.Select(y => y.Role.Name),
                    x.IsActive
                })
                .ToList())
                .Select(x => new RegisterModel()
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Roles = string.Join(", ", x.Roles),
                    SelectedRoleId = x.Roles.First()
                });

            var model = items.ToList();
            ViewBag.RoleId = new SelectList(roleRepository.Get.ToList(), "Name", "Name");



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
                        var defaultCoach = await UserManager.FindByNameAsync("RobertSikes");

                    }
                    return RedirectToAction("Index");
                }
                ViewBag.ErrorMessage = result.Errors.First();
                ViewData["EditError"] = result.Errors.First();
            }
            else
            {
                ViewData["EditError"] = "Please correct all errors";
            }

            //TODO: Testing theory about the errors.  Trying to resolve this.
            var userQuery = UserManager.Users.Where(x => x.IsActive == true).Include(r => r.Roles);
            var items = (userQuery
                .OrderBy(x => x.UserName)
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    x.FirstName,
                    x.LastName,
                    x.Email,
                    Roles = x.Roles.Select(y => y.Role.Name),
                    x.IsActive
                })
                .ToList())
                .Select(x => new RegisterModel()
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Roles = string.Join(", ", x.Roles),
                    SelectedRoleId = x.Roles.First()
                });

            var model = items.ToList();
            ViewBag.RoleId = new SelectList(roleRepository.Get.ToList(), "Name", "Name");



            return PartialView("_GridViewPartial", model);
        }

        public async Task<ActionResult> GridViewPartialEdit(EditUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                var systemRoles = new HashSet<Role>();
                var userEdit = await UserManager.FindByNameAsync(model.UserName);

                if(userEdit == null)
                {
                    return HttpNotFound();
                }

                userEdit.FirstName = model.FirstName;
                userEdit.LastName = model.LastName;
                userEdit.Email = model.Email;


                if (model.Password != "" && model.Password == model.ConfirmPassword)
                {
                    userEdit.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
                }
                
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

            //TODO: Testing theory about the errors.  Trying to resolve this.
            var userQuery = UserManager.Users.Where(x => x.IsActive == true).Include(r => r.Roles);
            var items = (userQuery
                .OrderBy(x => x.UserName)
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    x.FirstName,
                    x.LastName,
                    x.Email,
                    Roles = x.Roles.Select(y => y.Role.Name),
                    x.IsActive
                })
                .ToList())
                .Select(x => new RegisterModel()
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Roles = string.Join(", ", x.Roles),
                    SelectedRoleId = x.Roles.First()
                });

            var users = items.ToList();
            ViewBag.RoleId = new SelectList(roleRepository.Get.ToList(), "Name", "Name");

            return PartialView("_GridViewPartial", users);
        }


    }
}