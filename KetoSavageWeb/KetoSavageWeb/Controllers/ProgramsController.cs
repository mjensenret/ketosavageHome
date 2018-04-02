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
using System.Data.Entity;

namespace KetoSavageWeb.Controllers
{
    public class ProgramsController : KSUserManagedController<ProgramTemplate,ProgramRepository,ProgramEditViewModel>
    {
        private ProgramRepository programRepository;

        public ProgramsController(ProgramRepository pr)
            : base(pr, x => x.Name)
        {
            programRepository = pr;
        }
        // GET: Programs
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult programGridView(ProgramListViewModel model)
        {
            var query = model.ShowInactive ? this.programRepository.Get : this.programRepository.GetActive;

            var items = (query
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
                    x.WeightWeek,
                    Goal = x.Goal,
                    GoalName = x.Goal.Name
                })
                .ToList()
                .Select(x => new ProgramViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.programDescription,
                    WeightFactor = x.WeightWeek,
                    GoalName = x.Goal.Name,
                    GoalId = x.Goal.Id

                }
                ));

            model.Items = items;

            ViewBag.Goals = new SelectList(programRepository.getGoals(), "Id", "Name");

            return PartialView("_programGridViewPartial", model);
        }

        public async Task<ActionResult> programGridAdd(ProgramViewModel item)
        {
            if (ModelState.IsValid)
            {
                //var goal = programRepository.getGoalById(Convert.ToInt32(item.SelectedGoalId));
                
                var newProgram = new ProgramTemplate
                {
                    Name = item.Name,
                    programDescription = item.Description,
                    GoalId = item.GoalId,
                    WeightWeek = item.WeightFactor,
                    IsActive = true,
                    IsDeleted = false,
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                    CreatedBy = CurrentUser.UserName,
                    LastModifiedBy = CurrentUser.UserName
                };

                var result = await programRepository.CreateAsync(newProgram);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("programGridView");
            };
        }

        public async Task<ActionResult> programGridEdit(ProgramViewModel model)
        {
            if (ModelState.IsValid)
            {
                var editProgram = await programRepository.FindAsync(model.Id);
                if (editProgram == null)
                {
                    return HttpNotFound();
                }

                editProgram.Name = model.Name;
                editProgram.WeightWeek = model.WeightFactor;
                editProgram.programDescription = model.Description;
                editProgram.LastModified = DateTime.Now;
                editProgram.LastModifiedBy = CurrentUser.UserName;

                try
                {
                    programRepository.Update(editProgram);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message.ToString());
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed editing the program template!");
            return RedirectToAction("Index");
        }

        protected override ProgramEditViewModel createViewModel(ProgramTemplate entity)
        {
            return new ProgramEditViewModel { Program = entity };
        }

        protected override void updateEntity(ProgramTemplate entity, ProgramEditViewModel model)
        {
            entity.Name = model.Name;
            entity.programDescription = model.Description;
            entity.GoalId = model.GoalId;
            entity.IsActive = model.IsActive;
            entity.WeightWeek = model.WeightFactor;
        }

        //protected override void updateEntity(ProgramTemplate entity, ProgramTemplate model)
        //{
        //    entity.Name = model.Name;
        //    entity.IsActive = model.IsActive;
        //    entity.goals = model.goals;
        //}
    }


}