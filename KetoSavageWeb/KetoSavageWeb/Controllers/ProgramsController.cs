using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using KetoSavageWeb.ViewModels;
using KetoSavageWeb.Controllers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KetoSavageWeb.Infrastructure;
using System.Collections;

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
                    x.goals,
                    GoalName = x.goals.Name
                })
                .ToList()
                .Select(x => new ProgramListViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.programDescription,
                    SelectedGoalId = Convert.ToString(x.goals),
                    ProgramGoal = x.GoalName
                }
                ));

            //ViewBag.Goals = programRepository.GetAll.ToSelectList(
            //    items.Pro
            //    s => new SelectListData { Id = s.Id, Name = s.Name, IsActive = s.IsActive, IsDeleted = s.IsDeleted }
            //);
            ViewBag.Goals = new SelectList(programRepository.Get.ToList(), "Name", "Name");
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
                return RedirectToAction("programGridView");
            };
        }

        //protected override void updateEntity(ProgramTemplate entity, ProgramTemplate model)
        //{
        //    entity.Name = model.Name;
        //    entity.IsActive = model.IsActive;
        //    entity.goals = model.goals;
        //}
    }


}