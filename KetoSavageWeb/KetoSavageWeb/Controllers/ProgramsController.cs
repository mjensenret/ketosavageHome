using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using KetoSavageWeb.ViewModels;
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
        private ProgramRepository programRepository;

        public ProgramsController(ProgramRepository pr)
        {
            programRepository = pr;
        }
        // GET: Programs
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult programGridView()
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
                    goalName = x.goals.Name
                })
                .ToList()
                .Select(x => new ProgramListViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.programDescription,
                    ProgramGoal = x.goalName

                }
                ));
            var model = items.ToList();
            return PartialView("_programGridViewPartial", model);
        }

        public async Task<ActionResult> programGridAdd(ProgramEditViewModel item)
        {
            if (ModelState.IsValid)
            {
                var newProgram = new ProgramTemplate
                {
                    Name = item.Name,
                    programDescription = item.Description,
                    goals = item.Goal
                };

                var result = await programRepository.CreateAsync(newProgram);
                return RedirectToAction("Index");
            }
            else
            {
                RedirectToAction("programGridView");
            };
            RedirectToAction("programGridView");
        }
    }


}