using KetoSavageWeb.Interfaces;
using KetoSavageWeb.Models;
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
        private RoleRepository roleRepository;

        public ClientProgramsController(ProgramRepository pr, RoleRepository rr)
        {
            this.program = pr;
            this.roleRepository = rr;
        }
        // GET: Programs
        public async Task<ActionResult> Index()
        {

            //var roles = (from r in RoleManager.Roles where r.Name.Contains("Client") select r).FirstOrDefault();
            //var users = UserManager.Users.Where(x => x.Roles.Select(y => y.Role.Name.Contains("Coach")).ToList());
            var clients = from c in UserManager.Users
                          join p in program.GetByType(ProgramType.Coached) on c.Id equals p.ApplicationUserId
                          where c.Roles.Select(x => x.Role.Name).Contains("Client")
                            select new
                            {
                                c.UserName,
                                c.FirstName,
                                c.LastName,
                                p.startDate,
                                p.endDate
                            }
                            
                          ;
            
            var qry = (clients
                .OrderBy(x => x.UserName)
                .ToList())
                .Select(
                x => new ClientListViewModel
                {
                    FullName = string.Join(" ", x.FirstName, x.LastName),
                    userName = x.UserName,
                    currentProgramStartDate = x.startDate,
                    currentProgramEndDate = x.endDate

                });

            var model = qry.ToList();


            //var qry = (programs
            //    .OrderBy(x => x.ProgramUser.UserName)
            //    .ToList())
            //    .Select(
            //    x => new ClientListViewModel()
            //    {
            //        FullName = string.Join(" ", x.ProgramUser.FirstName, x.ProgramUser.LastName),
            //        currentProgramStartDate = x.startDate,
            //        currentProgramEndDate = x.endDate
            //    });


            //var model = qry;

            ////ViewData["Clients"] = users.ToList();
            //string userName = UserManager.Users.First().UserName.ToString();
            //var programs = program.GetActive.Where(x => x is CoachedPrograms);

            //return View("Index", userName);
            return PartialView("_clientGridViewPartial", model);
        }

        public ActionResult ClientList()
        {
            //var roles = (from r in RoleManager.Roles where r.Name.Contains("Client") select r).FirstOrDefault();
            //var users = UserManager.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(roles.Id));

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
            //var items = (users
            //    .OrderBy(x => x.UserName)
            //    .ToList())
            //    .Select(
            //        x => new ClientListViewModel
            //        {
            //            userName = x.UserName,
            //            FullName = string.Format("{0} {1}", x.FirstName, x.LastName)
            //        });

            //var model = users.ToList();
            //return PartialView("_clientList", model);
            return View();
        }
        public ActionResult ClientPrograms()
        {
            var clientPrograms = program.GetActive.Where(x => x is CoachedPrograms);
            return View();
        }
    }
}