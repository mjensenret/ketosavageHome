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
            var users = from user in UserManager.Users
                        from role in RoleManager.Roles.Where(y => y.Name == "Client")
                        select new
                        {
                            user.UserName,
                            user.FirstName,
                            user.LastName,
                            user.Email,
                            role.Name
                        };
            var model = users.ToList();

            var programs = program.GetActive.Where(x => x is CoachedPrograms);
            return View();
        }

        public ActionResult ClientPrograms()
        {
            var clientPrograms = program.GetActive.Where(x => x is CoachedPrograms);
            return View();
        }
    }
}