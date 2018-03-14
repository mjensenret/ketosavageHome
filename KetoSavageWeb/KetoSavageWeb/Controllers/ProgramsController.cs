using KetoSavageWeb.Repositories;
using KetoSavageWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb.Controllers
{
    public class ProgramsController : KSBaseController
    {
        private ProgramRepository programRepository;

        public ProgramsController(ProgramRepository pr)
        {
            programRepository = pr;
        }
        // GET: Programs
        public ActionResult Index()
        {
            var programQuery = programRepository.GetActive;
            var items = (programQuery
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.programDescription,
                    x.Created,
                    x.CreatedBy,
                    x.LastModified,
                    x.LastModifiedBy,
                    x.goals
                })
                .ToList()
                .Select(x => new ProgramListViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.programDescription,
                    ProgramGoal = x.goals.ToString()

                }
                ));
            var model = items.ToList();
            return View();
        }
    }
}