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
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;

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

        public PartialViewResult programGridView()
        {
            
            return PartialView("_programGridViewPartial");
        }

        public JsonResult getProgramList()
        {
            var query = programRepository.GetActive;

            var items = (query
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,
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
                        Description = x.Description,
                        WeightFactor = x.WeightWeek,
                        GoalName = x.Goal.Name,
                        GoalId = x.Goal.Id,
                        ProgramList = new SelectList(programRepository.getGoals(), "Id", "Name")

                    }
                ));


            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getProgramGoals()
        {
            var data = programRepository.getGoals().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> programGridAdd(ProgramViewModel item)
        {
            if (ModelState.IsValid)
            {
                //var goal = programRepository.getGoalById(Convert.ToInt32(item.SelectedGoalId));
                
                var newProgram = new ProgramTemplate
                {
                    Name = item.Name,
                    Description = item.Description,
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

        //[System.Web.Mvc.HttpPost]
        //public async Task<ActionResult> updateProgram(FormDataCollection form)
        //{
        //    var values = form.Get("values");

        //    if (ModelState.IsValid)
        //    {
        //        var editProgram = await programRepository.FindAsync(model.Id);
        //        if (editProgram == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        editProgram.Name = model.Name;
        //        editProgram.WeightWeek = model.WeightFactor;
        //        editProgram.programDescription = model.Description;
        //        editProgram.LastModified = DateTime.Now;
        //        editProgram.LastModifiedBy = CurrentUser.UserName;

        //        try
        //        {
        //            programRepository.Update(editProgram);
        //            return RedirectToAction("Index");
        //        }
        //        catch (Exception e)
        //        {
        //            ModelState.AddModelError("", e.Message.ToString());
        //        }
        //        return RedirectToAction("Index");
        //    }
        //    ModelState.AddModelError("", "Something failed editing the program template!");
        //    return RedirectToAction("Index");
        //}

        protected override ProgramEditViewModel createViewModel(ProgramTemplate entity)
        {
            return new ProgramEditViewModel { Program = entity };
        }

        protected override void updateEntity(ProgramTemplate entity, ProgramEditViewModel model)
        {
            entity.Name = model.Name;
            entity.Description = model.Description;
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

    public class ProgramsApiController : ApiController
    {
        private KSDataContext _context = new KSDataContext();

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetPrograms(DataSourceLoadOptions loadOptions)
        {
            var programs = _context.Programs.Where(x => x.IsActive);
            return Request.CreateResponse(DataSourceLoader.Load(programs, loadOptions));
        }
    }


}