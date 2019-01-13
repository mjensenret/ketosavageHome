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
    public class UserProgressController : KSBaseController
    {
        private ProgramRepository program;
        private RoleRepository roleRepository;
        private UserProgramRepository userProgramRepository;
        private DateRepository dateRepository;

        private KSDataContext _context = new KSDataContext();

        public UserProgressController(ProgramRepository pr, RoleRepository rr, UserProgramRepository up, DateRepository dr)
        {
            this.program = pr;
            this.roleRepository = rr;
            this.userProgramRepository = up;
            this.dateRepository = dr;
        }
        // GET: UserProgress
        public ActionResult Index()
        {
            var userId = CurrentUser.Id;
            Session["UserId"] = userId;
            return View();
        }

        public PartialViewResult EnterMacroForm()
        {
            var model = userProgramRepository.GetDailyProgressByDate(CurrentUser.Id, DateTime.Now.Date);
            var userId = CurrentUser.Id;

            EnterMacroViewModel viewModel = new EnterMacroViewModel();
            
            viewModel.macroUserId = userId;
            viewModel.macroDate = model.Dates.Date;
            viewModel.actualFat = (model.ActualFat != null) ? Convert.ToDouble(model.ActualFat) : 0 ;
            viewModel.actualProtein = (model.ActualProtein != null) ? Convert.ToDouble(model.ActualProtein) : 0;
            viewModel.actualCarb = (model.ActualCarbohydrate != null) ? Convert.ToDouble(model.ActualCarbohydrate) : 0;
            viewModel.hungerLevel = model.HungerLevel;
            viewModel.Notes = model.Notes;

            return PartialView("_enterDailyMacros", viewModel);
        }

        public JsonResult onMacroDateChange(string date)
        {
            try
            {
                DateTime selectedDate = Convert.ToDateTime(date);
                var model = userProgramRepository.GetDailyProgressByDate(CurrentUser.Id, Convert.ToDateTime(date));

                var data = new {
                    IsSuccess = true,
                    fat = model.ActualFat,
                    protein = model.ActualProtein,
                    carbs = model.ActualCarbohydrate,
                    notes = model.Notes,
                    hungerLevel = model.HungerLevel
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var data = new { IsSuccess = false, Message = "Something failed" };
                TempData.Remove("MacroDate");
                return Json(data);
            }

        }
        [HttpPost]
        public ActionResult UpdateActualMacros(EnterMacroViewModel model)
        {
            var userProgram = userProgramRepository.GetActive.Where(p => p.ProgramUserId == model.macroUserId).Include(d => d.DailyProgress).FirstOrDefault();
            if (userProgram.DailyProgress != null)
            {
                var dailyProgress = userProgram.DailyProgress.Where(dp => dp.Dates.Date == model.macroDate.Date).First();
                dailyProgress.ActualFat = model.actualFat;
                dailyProgress.ActualProtein = model.actualProtein;
                dailyProgress.ActualCarbohydrate = model.actualCarb;
                dailyProgress.HungerLevel = model.hungerLevel;
                dailyProgress.Notes = model.Notes;
                dailyProgress.LastModified = DateTime.Now;
                dailyProgress.LastModifiedBy = CurrentUser.UserName;

            }
            userProgramRepository.Update(userProgram);

            return RedirectToAction("Index");
        }
        public PartialViewResult EnterMeasurementsForm()
        {
            EnterMeasurementViewModel model = new EnterMeasurementViewModel();
            var userId = CurrentUser.Id;
            model.measurementUserId = userId;
            model.measurementDate = DateTime.Now;

            return PartialView("_enterDailyMeasurements", model);
        }
        [HttpPost]
        public ActionResult UpdateMeasurements(EnterMeasurementViewModel model)
        {
            var userProgram = userProgramRepository.GetActive.Where(p => p.ProgramUserId == model.measurementUserId).Include(d => d.DailyProgress).FirstOrDefault();
            if (userProgram.DailyProgress != null)
            {
                var dailyProgress = userProgram.DailyProgress.Where(dp => dp.Dates.Date == model.measurementDate.Date).First();
                dailyProgress.ActualWeight = model.actualWeight;
                dailyProgress.LastModified = DateTime.Now;
                dailyProgress.LastModifiedBy = CurrentUser.UserName;

            }
            userProgramRepository.Update(userProgram);

            return RedirectToAction("Index");

        }

        public ActionResult PastPerformanceGrid()
        {
            
            var currentDate = DateTime.Now;
            
            var _userId = CurrentUser.Id;
            //var programDetails = userProgramRepository.GetPastProgressByUser(_userId, currentDate);
            var programDetails = userProgramRepository.GetPastProgressByUser(_userId, currentDate);


            if (programDetails.Count() > 0)
            {
                var q = (programDetails
                    .OrderBy(x => x.Dates.Date)
                    .Select(x => new
                    {
                        x.Id,
                        x.UserProgram.ProgramUserId,
                        x.Dates,
                        x.PlannedWeight,
                        x.ActualWeight,
                        x.PlannedFat,
                        x.ActualFat,
                        x.PlannedProtein,
                        x.ActualProtein,
                        x.PlannedCarbohydrate,
                        x.ActualCarbohydrate
                    })
                    .ToList()
                    .Select(y => new UserProgramProgress
                    {
                        Id = y.Id,
                        UserId = y.ProgramUserId,
                        Date = y.Dates.Date,
                        WeekDay = y.Dates.WeekDayName,
                        PlannedWeight = y.PlannedWeight,
                        ActualWeight = y.ActualWeight,
                        PlannedFat = y.PlannedFat,
                        ActualFat = y.ActualFat,
                        PlannedCarbohydrates = y.PlannedCarbohydrate,
                        ActualCarbohydrates = y.ActualCarbohydrate,
                        PlannedProtein = y.PlannedProtein,
                        ActualProtein = y.ActualProtein
                    })
                    );

                return PartialView("_pastPerformanceGrid", q);
            }
            else
            {
                return PartialView("_pastPerformanceGrid");
            }
        }
        [HttpPost]
        public ActionResult EntryPopup(string buttonName)
        {
            if (buttonName == "macros")
            {
                return RedirectToAction("EnterMacroForm");
            }
            else
            {
                return RedirectToAction("EnterMeasurementsForm");
            }

        }
    }
}