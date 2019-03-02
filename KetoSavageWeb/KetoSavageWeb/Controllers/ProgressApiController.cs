using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using KetoSavageWeb.ViewModels;
using Newtonsoft.Json;

namespace KetoSavageWeb.Controllers
{
    public class ProgressApiController : ApiController
    {
        private ProgramRepository program;
        private RoleRepository roleRepository;
        private UserProgramRepository userProgramRepository;
        private DateRepository dateRepository;

        private KSDataContext _context = new KSDataContext();

        public ProgressApiController()
        {

        }

        public ProgressApiController(ProgramRepository pr, RoleRepository rr, UserProgramRepository up, DateRepository dr)
        {
            this.program = pr;
            this.roleRepository = rr;
            this.userProgramRepository = up;
            this.dateRepository = dr;
        }

        //[HttpGet]
        //public HttpResponseMessage GetMacroEntry(DateTime date, int userId, DataSourceLoadOptions loadOptions)
        //{
        //    var query = _context.DailyProgress.Where(x => x.IsActive && !x.IsDeleted && x.Dates.Date == date.Date && x.UserProgram.ProgramUser.Id == userId);

        //    var model = query
        //        .Select(x => new
        //        {
        //            x.Id,
        //            x.Dates.Date,
        //            x.DateId,
        //            x.ActualFat,
        //            x.ActualProtein,
        //            x.ActualCarbohydrate,
        //            x.Notes,
        //            x.HungerLevelId,
        //            userId
        //        })
        //        .ToList()
        //        .Select(y => new EnterMacroViewModel()
        //        {
        //            macroUserId = y.userId,
        //            macroDate = y.Date,
        //            macroProgressId = y.Id,
        //            actualFat = Convert.ToDouble(y.ActualFat),
        //            actualProtein = Convert.ToDouble(y.ActualProtein),
        //            actualCarb = Convert.ToDouble(y.ActualCarbohydrate),
        //            Notes = y.Notes,
        //            hungerLevelId = y.HungerLevelId.HasValue ? Convert.ToInt32(y.HungerLevelId) : 0
        //        });

        //    return Request.CreateResponse(DataSourceLoader.Load(model, loadOptions));
        //}

        //[Route("api/Measurements/{action}", Name = "Measurements")]
        //[HttpGet]
        //public HttpResponseMessage GetMeasurementDetails(int measurementId, DataSourceLoadOptions loadOptions)
        //{
        //    IQueryable<MeasurementEntriesViewModel> measurementDetailsViewModel = null;

        //    measurementDetailsViewModel =
        //        _context.MeasurementDetail.Where(x => x.measurementHeaderId == measurementId).Select(x => new MeasurementEntriesViewModel()
        //        {
        //            MeasurementId = x.measurementHeaderId,
        //            MeasurementType = x.measurementType,
        //            MeasurementValue = x.measurementValue
        //        });
           
        //    return Request.CreateResponse(DataSourceLoader.Load(measurementDetailsViewModel, loadOptions));
        //}

        [HttpGet]
        public HttpResponseMessage GetMeasurements(DateTime date, DataSourceLoadOptions loadOptions)
        {
            var m = _context.MeasurementHeader.Where(x => x.Dates.Date == date).Include(x => x.MeasurementDetails);

            var model = m.Select(x => new
            {
                x.Id,
                x.Dates.Date,
                x.MeasurementNotes,
                x.MeasurementDetails
            })
            .ToList()
            .Select(x => new TestMeasurementViewModel()
            {
                Id = x.Id,
                MeasurementDate = x.Date,
                MeasurementNotes = x.MeasurementNotes,
                MeasurementDetails = x.MeasurementDetails
            });

            return Request.CreateResponse(DataSourceLoader.Load(model, loadOptions));
        }

        [HttpGet]
        public HttpResponseMessage GetUserProgressGrid(int userId, DataSourceLoadOptions loadOptions)
        {
            var query = _context.DailyProgress.Where(x => x.UserProgram.ProgramUserId == userId && x.UserProgram.IsActive && !x.UserProgram.IsDeleted);

            var model = query
                .Select(x => new
                {
                    x.Dates.DateKey,
                    x.Dates.Date,
                    x.Dates.WeekDayName,
                    x.Id,
                    x.UserProgramId,
                    x.PlannedWeight,
                    x.ActualWeight,
                    x.PlannedFat,
                    x.ActualFat,
                    x.PlannedProtein,
                    x.ActualProtein,
                    x.PlannedCarbohydrate,
                    x.ActualCarbohydrate,
                    x.IsRefeed
                })
                .ToList()
                .Select(x => new UserProgramProgress()
                {
                    Id = x.Id,
                    IsRefeed = x.IsRefeed,
                    UserId = x.UserProgramId,
                    DateId = x.DateKey,
                    Date = x.Date,
                    WeekDay = x.WeekDayName,
                    PlannedWeight = x.PlannedWeight,
                    ActualWeight = x.ActualWeight,
                    PlannedFat = x.PlannedFat,
                    ActualFat = x.ActualFat,
                    PlannedProtein = x.PlannedProtein,
                    ActualProtein = x.ActualProtein,
                    PlannedCarbohydrates = x.PlannedCarbohydrate,
                    ActualCarbohydrates = x.ActualCarbohydrate
                });

            return Request.CreateResponse(DataSourceLoader.Load(model, loadOptions));
        }

        [HttpPut]
        public HttpResponseMessage UpdateSingleDay(FormDataCollection form)
        {
            var key = Convert.ToDateTime(form.Get("key"));
            var values = form.Get("values");
            var updDailyProgressVm = new UserProgramProgress();
            JsonConvert.PopulateObject(values, updDailyProgressVm);

            var updDailyProgress = _context.DailyProgress.Single(x => x.Id == updDailyProgressVm.Id);
            updDailyProgress.IsRefeed = updDailyProgressVm.IsRefeed;
            if (updDailyProgressVm.PlannedFat != null)
                updDailyProgress.PlannedFat = updDailyProgressVm.PlannedFat;
            if (updDailyProgressVm.PlannedCarbohydrates != null)
                updDailyProgress.PlannedCarbohydrate = updDailyProgressVm.PlannedCarbohydrates;
            if (updDailyProgressVm.PlannedProtein != null)
                updDailyProgress.PlannedProtein = updDailyProgressVm.PlannedProtein;

            Validate(updDailyProgress);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "ModelState is invalid");

            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public HttpResponseMessage GetPreviousPvA(int userId, DataSourceLoadOptions loadOptions)
        {
            var date = DateTime.Now;
            var previousWeek = _context.DateModels.Where(x => x.Date == date.Date).Select(y => y.ISOWeekOfYear).First() - 1;
            var dailyProgresses = _context.DailyProgress.Where(x => x.Dates.ISOWeekOfYear == previousWeek && x.Dates.Year == date.Year);

            var model = dailyProgresses.Select(x => new
            {
                x.PlannedFat,
                x.PlannedProtein,
                x.PlannedCarbohydrate,
                x.ActualFat,
                x.ActualProtein,
                x.ActualCarbohydrate
            })
            .ToList();

            var plannedFat = Convert.ToInt32(model.Average(x => x.PlannedFat));
            var plannedProtein = Convert.ToInt32(model.Average(x => x.PlannedProtein));
            var plannedCarbs = Convert.ToInt32(model.Average(x => x.PlannedCarbohydrate));
            var actualFat = Convert.ToInt32(model.Average(x => x.ActualFat));
            var actualProtein = Convert.ToInt32(model.Average(x => x.ActualProtein));
            var actualCarbs = Convert.ToInt32(model.Average(x => x.ActualCarbohydrate));

            List<PvAMacroPieChart> pie = new List<PvAMacroPieChart>()
            {
                new PvAMacroPieChart { macro = "Fat", Planned = plannedFat, Actual = actualFat },
                new PvAMacroPieChart { macro = "Protein", Planned = plannedProtein, Actual = actualProtein},
                new PvAMacroPieChart { macro = "Carbs", Planned = plannedCarbs, Actual = actualCarbs}
            };

            return Request.CreateResponse(DataSourceLoader.Load(pie, loadOptions));

        }

        [HttpGet]
        public HttpResponseMessage GetCurrentPvA(int userId, DataSourceLoadOptions loadOptions)
        {
            var date = DateTime.Now;
            var currentWeek = _context.DateModels.Where(x => x.Date == date.Date).Select(y => y.ISOWeekOfYear).First();
            var dailyProgresses = _context.DailyProgress.Where(x => x.Dates.ISOWeekOfYear == currentWeek && x.Dates.Year == date.Year);

            var model = dailyProgresses.Select(x => new
            {
                x.PlannedFat,
                x.PlannedProtein,
                x.PlannedCarbohydrate,
                x.ActualFat,
                x.ActualProtein,
                x.ActualCarbohydrate
            })
            .ToList();

            var plannedFat = Convert.ToInt32(model.Average(x => x.PlannedFat));
            var plannedProtein = Convert.ToInt32(model.Average(x => x.PlannedProtein));
            var plannedCarbs = Convert.ToInt32(model.Average(x => x.PlannedCarbohydrate));
            var actualFat = Convert.ToInt32(model.Average(x => x.ActualFat));
            var actualProtein = Convert.ToInt32(model.Average(x => x.ActualProtein));
            var actualCarbs = Convert.ToInt32(model.Average(x => x.ActualCarbohydrate));

            List<PvAMacroPieChart> pie = new List<PvAMacroPieChart>()
            {
                new PvAMacroPieChart { macro = "Fat", Planned = plannedFat, Actual = actualFat },
                new PvAMacroPieChart { macro = "Protein", Planned = plannedProtein, Actual = actualProtein},
                new PvAMacroPieChart { macro = "Carbs", Planned = plannedCarbs, Actual = actualCarbs}
            };

            return Request.CreateResponse(DataSourceLoader.Load(pie, loadOptions));

        }


    }
}
