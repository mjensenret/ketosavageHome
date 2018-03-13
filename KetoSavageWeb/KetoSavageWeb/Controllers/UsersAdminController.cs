using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections;
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
        private RoleRepository roleRepository;

        ApplicationUserManager _userManager;
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

        //public ApplicationRoleManager RoleManager
        //{
        //    get
        //    {
        //        return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
        //    }
        //    private set
        //    {
        //        _roleManager = value;
        //    }
        //}

        public UsersAdminController()
        {

        }
        public UsersAdminController(ApplicationUserManager userManager, RoleRepository rr)
        {
            UserManager = userManager;
            roleRepository = rr;
        }


        // GET: UsersAdmin

        public async Task<ActionResult> Index(ListModel<UserListViewModel> model)
        {
            var userQuery = UserManager.Users;
            var systemRoles = new HashSet<Role>();

            var items = (await userQuery
                .OrderBy(x => x.UserName)
                //.Page(model.Page, model.ItemsPerPage, model.TotalItems)
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    x.Email,
                    x.FirstName,
                    x.LastName,
                    x.Roles

                })
                .ToListAsync())
                .Select(x => new UserListViewModel
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Roles = string.Join(", ", systemRoles.Where(userRole => x.Roles.Select(r => r.RoleId).Contains(userRole.Id)).Select(r => r.Name).ToList())
                    //Roles = string.Join (", ", systemRoles.Wh)
                });

            model.Items = items;

            return View(model);
        }

        public ActionResult userGridPartialView(ListModel<UserListViewModel> model)
        {
            var userQuery = UserManager.Users;
            var systemRoles = new HashSet<Role>();

            var items = (userQuery
                .OrderBy(x => x.UserName)
                //.Page(model.Page, model.ItemsPerPage, model.TotalItems)
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    x.Email,
                    x.FirstName,
                    x.LastName,
                    x.Roles
                })
                .ToList())
                .Select(x => new UserListViewModel
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Roles = string.Join(", ", systemRoles.Where(userRole => x.Roles.Select(r => r.RoleId).Contains(userRole.Id)).Select(r => r.Name).ToList())
                });

            model.Items = items;


            return PartialView("UserGridPartialView", model);
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(RegisterModel model, params string[] selectedRoles)
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
                    if (model.SelectedRoleId != null)
                    {
                        var roleResult = await UserManager.AddToRolesAsync(user.Id, model.SelectedRoleId);
                        if (!roleResult.Succeeded)
                        {
                            ModelState.AddModelError("", roleResult.Errors.First());
                            //ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            ViewBag.RoleId = new SelectList(roleRepository.Get.ToList(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", result.Errors.First());

                    ViewBag.RoleId = new SelectList(roleRepository.Get.ToList(), "Name", "Name");
                    return View(model);
                }
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(roleRepository.Get.ToList(), "Name", "Name");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<ActionResult> UpdateUserPartial(string userName)
        {
            var user = await UserManager.FindByNameAsync(userName);
            var systemRoles = new HashSet<Role>();


            EditUserViewModel model = new EditUserViewModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = string.Join(", ", systemRoles.Where(userRole => user.Roles.Select(r => r.RoleId).Contains(userRole.Id)).Select(r => r.Name).ToList())
            };
            return PartialView("UpdateUserPartial");
        }

        [HttpPost]
        public async Task<ActionResult> UpdateUserPartial(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                //find user
                var userEdit = await UserManager.FindByIdAsync(model.Id);

                if (userEdit == null)
                {
                    return HttpNotFound();
                }

                userEdit.FirstName = model.FirstName;
                userEdit.LastName = model.LastName;
                userEdit.Email = model.Email;

                var userRole = await UserManager.GetRolesAsync(model.Id);

                await UserManager.RemoveFromRolesAsync(userEdit.Id, userRole.ToArray());

                var updRoleResult = await UserManager.AddToRoleAsync(userEdit.Id, model.SelectedRoleId);
                if (!updRoleResult.Succeeded)
                {
                    ModelState.AddModelError("", updRoleResult.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed!");
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult testUpdate(string userName)
        {
            var user = UserManager.Users.Where(x => x.UserName == userName).FirstOrDefault();
            var systemRoles = new HashSet<Role>();
            var userRoles = UserManager.GetRolesAsync(user.Id).Result.ToList();

            EditUserViewModel upd = new EditUserViewModel()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                //Roles = string.Join(", ", systemRoles.Where(userRole => user.Roles.Select(r => r.RoleId).Contains(userRole.Id)).Select(r => r.Name).ToList()),
                SelectedRoleId = userRoles.FirstOrDefault(),
                RolesList = roleRepository.Get.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })

            };

            return PartialView("UserGridPartialView", upd);

        }
        [HttpPost]
        public ActionResult UpdateUser([Bind(Include = "FirstName,LastName,Email,SelectedRoleId")] EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByNameAsync(model.UserName);
                return RedirectToAction("Index");
            }
            return View();

        }

        //public ActionResult GetUserGridPartialView(EditUserViewModel model)
        //{

        //}

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