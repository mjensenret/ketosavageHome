using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using KetoSavageWeb.ViewModels;
using Newtonsoft.Json;

namespace KetoSavageWeb.Controllers
{
    public class ProgramApiController : ApiController
    {
        KSDataContext _context = new KSDataContext();

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetAllPrograms(DataSourceLoadOptions loadOptions)
        {
            var query = _context.Programs.Where(x => x.IsActive);

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
                    x.Goal,
                    GoalName = x.Goal.Name,
                    x.HungerLevel
                })
                .ToList()
                .Select(x => new ProgramViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        WeightFactor = x.WeightWeek,
                        Goal = x.Goal,
                        GoalName = x.Goal.Name,
                        GoalId = x.Goal.Id

                    }
                ).ToList());

            return Request.CreateResponse(DataSourceLoader.Load(items, loadOptions));
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage getProgramGoals(DataSourceLoadOptions loadOptions)
        {
            var data = _context.ProgramGoals.ToList();
            return Request.CreateResponse(DataSourceLoader.Load(data, loadOptions));
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddProgram(FormDataCollection form)
        {
            var values = form.Get("values");
            var newProgramViewModel = new ProgramViewModel();
            JsonConvert.PopulateObject(values, newProgramViewModel);

            var newProgram = new ProgramTemplate();
            newProgram.Name = newProgramViewModel.Name;
            newProgram.Description = newProgramViewModel.Description;
            newProgram.GoalId = newProgramViewModel.GoalId;
            newProgram.Created = DateTime.Now;
            newProgram.CreatedBy = "TempTest";
            newProgram.LastModified = DateTime.Now;
            newProgram.LastModifiedBy = "TempModified";

            Validate(newProgram);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "ModelState is invalid");

            _context.Programs.Add(newProgram);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created);

        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateProgram(FormDataCollection form)
        {
            var key = Convert.ToInt32(form.Get("key"));
            var values = form.Get("values");
            ProgramTemplate program = _context.Programs.Single(p => p.Id == key);

            
            JsonConvert.PopulateObject(values, program);

            Validate(program);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid ModelState");

            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //Hunger Levels
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getHungerLevels(DataSourceLoadOptions loadOptions, int programId)
        {
            var hungerLevels = _context.Programs.Where(x => x.Id == programId).Select(y => y.HungerLevel).FirstOrDefault();
            return Request.CreateResponse(DataSourceLoader.Load(hungerLevels, loadOptions));
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage getHungerLevelsByUser(DataSourceLoadOptions loadOptions, int userId)
        {
            var masterProgramId = _context.UserPrograms.Where(x => x.ProgramUserId == userId && x.IsActive && !x.IsDeleted).Select(y => y.MasterProgram.Id).FirstOrDefault();
            var hungerLevels = _context.HungerLevels.Where(x => x.programId == masterProgramId);

            return Request.CreateResponse(DataSourceLoader.Load(hungerLevels, loadOptions));
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddHungerLevel(FormDataCollection form)
        {
            var key = Convert.ToInt32(form.Get("key"));
            var values = form.Get("values");
            var newHungerLevel = new HungerLevel();

            JsonConvert.PopulateObject(values, newHungerLevel);

            Validate(newHungerLevel);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Hunger level model state is invalid");

            _context.HungerLevels.Add(newHungerLevel);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateHungerLevel(FormDataCollection form)
        {
            var key = Convert.ToInt32(form.Get("key"));
            var values = form.Get("values");
            var updHungerLevels = _context.HungerLevels.Single(x => x.Id == key);

            JsonConvert.PopulateObject(values, updHungerLevels);

            Validate(updHungerLevels);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Hunger level model state is invalid");

            _context.SaveChanges();
            
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}