using KetoSavageWeb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb.Controllers
{
    public class ProgramsController : KSBaseController
    {
        private ProgramRepository program;

        public ProgramsController(ProgramRepository pr)
        {
            this.program = pr;
        }
        // GET: Programs
        public async Task<ActionResult> Index()
        {
            var programs = program.GetActive;
            return View();
        }
    }
}