using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using KetoSavageWeb.Controllers.Abstract;
using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;

namespace KetoSavageWeb.Controllers
{
    public class ManageUsersAPIController : KSBaseAPIController
    {
        KSDataContext _context = new KSDataContext();
        private UserProgramRepository userProgramRepository;
        private RoleRepository roleRepository;

        ManageUsersAPIController() : base() { }
        ManageUsersAPIController(UserProgramRepository up, RoleRepository rr)
        {
            userProgramRepository = up;
            roleRepository = rr;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetUserList(DataSourceLoadOptions loadOptions)
        {
            var userQuery = UserManager.Users.Where(x => x.IsActive == true).Include(x => x.Roles).ToList();
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
            return Request.CreateResponse(DataSourceLoader.Load(model, loadOptions));
        }

        [HttpGet]
        public HttpResponseMessage GetUserRoles(DataSourceLoadOptions loadOptions)
        {
            var roles = RoleManager.Roles.ToList();
            return Request.CreateResponse(DataSourceLoader.Load(roles, loadOptions));
        }
        [HttpPost]
        public async Task<HttpResponseMessage> AddUserAsync(FormDataCollection form)
        {
            var values = form.Get("values");

            var newUser = new RegisterModel();
            JsonConvert.PopulateObject(values, newUser);
            var newAppUser = new ApplicationUser();
            newAppUser.UserName = newUser.UserName;
            newAppUser.FirstName = newUser.FirstName;
            newAppUser.LastName = newUser.LastName;
            newAppUser.Email = newUser.Email;

            Validate(newAppUser);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Model Error while adding");

            var result = await UserManager.CreateAsync(newAppUser, newUser.Password);
            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(newAppUser.Id, newUser.SelectedRoleId);
                if (newUser.SelectedRoleId == "Client")
                {
                    var defaultCoach = await UserManager.FindByNameAsync("RobertSikes");

                }

                return Request.CreateResponse(HttpStatusCode.Created);
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error saving new user");
        }

        [HttpPut]
        public async Task<HttpResponseMessage> UpdateUserAsync(FormDataCollection form)
        {
            var values = form.Get("values");
            var key = Convert.ToInt32(form.Get("key"));
            var editUser = new EditUserViewModel();
            JsonConvert.PopulateObject(values, editUser);

            var user = await UserManager.FindByIdAsync(key);

            if (editUser.FirstName != null)
                user.FirstName = editUser.FirstName;
            if (editUser.LastName != null)
                user.LastName = editUser.LastName;
            if (editUser.Email != null)
                user.Email = editUser.Email;
            if (editUser.Password != null && editUser.Password == editUser.ConfirmPassword)
            {
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(editUser.Password);
            }
            if (editUser.SelectedRoleId != null)
            {
                var role = await UserManager.GetRolesAsync(key);
                await UserManager.RemoveFromRolesAsync(key, role.ToArray());

                var updRoleResult = await UserManager.AddToRoleAsync(key, editUser.SelectedRoleId);
            }
            var result = await UserManager.UpdateAsync(user);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteUser(FormDataCollection form)
        {
            var key = Convert.ToInt32(form.Get("key"));

            var deleteUser = await UserManager.FindByIdAsync(key);
            deleteUser.IsActive = false;
            deleteUser.IsDeleted = true;
            deleteUser.LastModified = DateTime.Now;
            deleteUser.LastModifiedBy = RequestContext.Principal.Identity.Name;

            var result = await UserManager.UpdateAsync(deleteUser);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
