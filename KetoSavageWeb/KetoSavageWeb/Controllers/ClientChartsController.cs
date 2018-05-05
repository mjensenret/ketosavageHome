using DevExpress.Web.Mvc;
using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using KetoSavageWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb.Controllers
{
    public class ClientChartsController : KSBaseController
    {
        private ProgramRepository program;
        private RoleRepository roleRepository;
        private UserProgramRepository userProgramRepository;
        private DateRepository dateRepository;

        private KSDataContext _context = new KSDataContext();

        public ClientChartsController(ProgramRepository pr, RoleRepository rr, UserProgramRepository up, DateRepository dr)
        {
            this.program = pr;
            this.roleRepository = rr;
            this.userProgramRepository = up;
            this.dateRepository = dr;
        }
        // GET: UserProgress
        public PartialViewResult WeightGraph()
        {
            
            return PartialView("_clientWeightGraph");
        }


        //    public PartialViewResult EnterMacroForm()
        //    {
        //        EnterMacroViewModel model = new EnterMacroViewModel();
        //        var userId = CurrentUser.Id;
        //        model.macroUserId = userId;
        //        model.macroDate = DateTime.Now;

        //        return PartialView("_enterDailyMacros", model);
        //    }
        //    [HttpPost]
        //    public ActionResult UpdateActualMacros(EnterMacroViewModel model)
        //    {
        //        var userProgram = userProgramRepository.GetActive.Where(p => p.ProgramUserId == model.macroUserId).Include(d => d.DailyProgress).FirstOrDefault();
        //        if (userProgram.DailyProgress != null)
        //        {
        //            var dailyProgress = userProgram.DailyProgress.Where(dp => dp.Dates.Date == model.macroDate.Date).First();
        //            dailyProgress.ActualFat = model.actualFat;
        //            dailyProgress.ActualProtein = model.actualProtein;
        //            dailyProgress.ActualCarbohydrate = model.actualCarb;
        //            dailyProgress.LastModified = DateTime.Now;
        //            dailyProgress.LastModifiedBy = CurrentUser.UserName;

        //        }
        //        userProgramRepository.Update(userProgram);

        //        return RedirectToAction("Index");
        //    }
        //    public PartialViewResult EnterMeasurementsForm()
        //    {
        //        EnterMeasurementViewModel model = new EnterMeasurementViewModel();
        //        var userId = CurrentUser.Id;
        //        model.measurementUserId = userId;
        //        model.measurementDate = DateTime.Now;

        //        return PartialView("_enterDailyMeasurements", model);
        //    }
        //    [HttpPost]
        //    public ActionResult UpdateMeasurements(EnterMeasurementViewModel model)
        //    {
        //        var userProgram = userProgramRepository.GetActive.Where(p => p.ProgramUserId == model.measurementUserId).Include(d => d.DailyProgress).FirstOrDefault();
        //        if (userProgram.DailyProgress != null)
        //        {
        //            var dailyProgress = userProgram.DailyProgress.Where(dp => dp.Dates.Date == model.measurementDate.Date).First();
        //            dailyProgress.ActualWeight = model.actualWeight;
        //            dailyProgress.LastModified = DateTime.Now;
        //            dailyProgress.LastModifiedBy = CurrentUser.UserName;

        //        }
        //        userProgramRepository.Update(userProgram);

        //        return RedirectToAction("Index");

        //    }

        //    public ActionResult PastPerformanceGrid()
        //    {

        //        var currentDate = DateTime.Now;

        //        var _userId = CurrentUser.Id;
        //        //var programDetails = userProgramRepository.GetPastProgressByUser(_userId, currentDate);
        //        var programDetails = userProgramRepository.GetPastProgressByUser(_userId, currentDate);


        //        if (programDetails.Count() > 0)
        //        {
        //            var q = (programDetails
        //                .Select(x => new
        //                {
        //                    x.Id,
        //                    x.UserProgram.ProgramUserId,
        //                    x.Dates,
        //                    x.PlannedWeight,
        //                    x.ActualWeight,
        //                    x.PlannedFat,
        //                    x.ActualFat,
        //                    x.PlannedProtein,
        //                    x.ActualProtein,
        //                    x.PlannedCarbohydrate,
        //                    x.ActualCarbohydrate
        //                })
        //                .ToList()
        //                .Select(y => new UserProgramProgress
        //                {
        //                    Id = y.Id,
        //                    UserId = y.ProgramUserId,
        //                    Date = y.Dates.Date,
        //                    WeekDay = y.Dates.WeekDayName,
        //                    PlannedWeight = y.PlannedWeight,
        //                    ActualWeight = y.ActualWeight,
        //                    PlannedFat = y.PlannedFat,
        //                    ActualFat = y.ActualFat,
        //                    PlannedCarbohydrates = y.PlannedCarbohydrate,
        //                    ActualCarbohydrates = y.ActualCarbohydrate,
        //                    PlannedProtein = y.PlannedProtein,
        //                    ActualProtein = y.ActualProtein
        //                })
        //                );

        //            return PartialView("_pastPerformanceGrid", q);
        //        }
        //        else
        //        {
        //            return PartialView("_pastPerformanceGrid");
        //        }
        //    }

        //    public ActionResult EntryPopup(string buttonName)
        //    {
        //        if (buttonName == "Macros")
        //        {
        //            return RedirectToAction("EnterMacroForm");
        //        }
        //        else
        //        {
        //            return RedirectToAction("EnterMeasurementsForm");
        //        }

        //    }

        public ActionResult ClientWeightGraph(int userId)
        {
            var currentDate = DateTime.Now;
            var plannedWeight = userProgramRepository.GetDailyProgressByUser(userId)
                .Select(x => new WeightViewModel()
                {
                    date = x.Dates.Date,
                    type = "Planned",
                    weight = x.PlannedWeight
                })
                .ToList();
            
            var actualWeight = userProgramRepository.GetDailyProgressByUser(userId)
                .Select(x => new WeightViewModel()
                {
                    date = x.Dates.Date,
                    type = "Actual",
                    weight = x.ActualWeight
                })
                .ToList();

            var weightChart = plannedWeight.Union(actualWeight).ToList();
            
            return PartialView("_clientWeightGraph", weightChart);
        }
    }
}