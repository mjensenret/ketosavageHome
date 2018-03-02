using KetoSavageWeb.Interfaces;
using KetoSavageWeb.Models;
using KetoSavageWeb.Models.Contexts;
using KetoSavageWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb.Controllers
{
    public class ClientProgramsController : KSBaseController
    {
        private ProgramRepository program;

        public ClientProgramsController(ProgramRepository pr)
        {
            this.program = pr;
        }
        // GET: Programs
        public async Task<ActionResult> Index()
        {

            var roles = (from r in RoleManager.Roles where r.Name.Contains("Client") select r).FirstOrDefault();
            var users = UserManager.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(roles.Id)).ToList();
            var programs = program.GetActive.Where(z => z is CoachedPrograms);

            var qry = (programs
                .OrderBy(x => x.ApplicationUser.FirstName)
                .ToList())
                .Select(
                x => new ClientListViewModel()
                {
                    FullName = string.Join(" ", x.ApplicationUser.FirstName, x.ApplicationUser.LastName),
                    currentProgramStartDate = x.startDate,
                    currentProgramEndDate = x.endDate
                });


            var model = qry;

            ViewData["Clients"] = users.ToList();
            string userName = UserManager.Users.First().UserName.ToString();
            //var programs = program.GetActive.Where(x => x is CoachedPrograms);

            return View("Index", userName);
        }

        public ActionResult ClientList()
        {
            var roles = (from r in RoleManager.Roles where r.Name.Contains("Client") select r).FirstOrDefault();
            var users = UserManager.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(roles.Id));

            //var model = new ClientListViewModel
            //{
            //    userName = users.User
            //}
            //var users = (from user in UserManager.Users
            //            from role in RoleManager.Roles
            //            where role.Name.Contains("Client")
            //            .ToList())
            //            .Select
            //            select new ClientListViewModel()
            //            {
            //                userName = user.UserName,
            //                FullName = string.Format("{0} {1}", user.FirstName, user.LastName)

            //            };
            var items = (users
                .OrderBy(x => x.UserName)
                .ToList())
                .Select(
                    x => new ClientListViewModel
                    {
                        userName = x.UserName,
                        FullName = string.Format("{0} {1}", x.FirstName, x.LastName)
                    });

            var model = users.ToList();
            return PartialView("_clientList", model);
        }
        public ActionResult ClientPrograms()
        {
            var clientPrograms = program.GetActive.Where(x => x is CoachedPrograms);
            return View();
        }
    }
}