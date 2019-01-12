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
                .OrderBy(x => x.date)
                .ToList();
            
            var actualWeight = userProgramRepository.GetDailyProgressByUser(userId)
                .Select(x => new WeightViewModel()
                {
                    date = x.Dates.Date,
                    type = "Actual",
                    weight = x.ActualWeight
                })
                .OrderBy(x => x.date)
                .ToList();

            var weightChart = plannedWeight.Union(actualWeight).ToList();
            
            return PartialView("_clientWeightGraph", weightChart);
        }

        public ActionResult ClientPerformanceGauge(int userId)
        {
            var model = userProgramRepository.calcWeightChangeByUser(userId);

            return PartialView("_clientPerformanceGauge", model);
        }

        public ActionResult ClientPastWeekMacroChart(int userId)
        {
            var currentDate = DateTime.Now;
            var previousWeekNumber = dateRepository.getLastWeekNumber(currentDate);

            var pastProgress = userProgramRepository.GetPastProgressByUser(userId, currentDate);


            var q = (pastProgress
                    .Where(x => x.Dates.WeekOfYear == previousWeekNumber)
                    .GroupBy(x => x.Dates.WeekOfYear))
                    .Select(x => new
                    {
                        avgActFat = x.Average(y => y.ActualFat),
                        avgActProt = x.Average(y => y.ActualProtein),
                        avgActCarb = x.Average(y => y.ActualCarbohydrate)
                    })
                    .ToArray();

            var fatCalories = q.Length > 0 ? ((q.First().avgActFat * 9)) : 0;
            var protCalories = q.Length > 0 ? ((q.First().avgActProt * 4)) : 0;
            var carbCalories = q.Length > 0 ? ((q.First().avgActCarb * 4)) : 0;

            var totalCalories = (fatCalories + protCalories + carbCalories);

            //TODO: Move this to the repository, add string for current/past/future
            List<WeeklyMacroPieChart> model = new List<WeeklyMacroPieChart>()
            {
                new WeeklyMacroPieChart
                {
                    macro = "Fat", value = Convert.ToDouble(fatCalories / totalCalories)
                },
                new WeeklyMacroPieChart
                {
                    macro = "Protein", value = Convert.ToDouble(protCalories / totalCalories)
                },
                new WeeklyMacroPieChart
                {
                    macro = "Carbs", value = Convert.ToDouble(carbCalories / totalCalories)
                }

            };


            return PartialView("_clientPastWeekMacroChart", model);


        }

        public ActionResult ClientCurrentWeekMacroGauge(int userId)
        {
            var currentDate = DateTime.Now;
            var currentWeekNumber = dateRepository.getCurrentWeekNumber(currentDate);

            var currentProgress = userProgramRepository.GetCurrentProgressByUser(userId, currentDate);


            var q = (currentProgress
                    .GroupBy(x => x.Dates.WeekOfYear))
                    .Select(x => new
                    {
                        avgPlannedFat = x.Average(y => y.PlannedFat),
                        avgPlannedProt = x.Average(y => y.PlannedProtein),
                        avgPlannedCarb = x.Average(y => y.PlannedCarbohydrate),
                        avgActualFat = x.Average(y => y.ActualFat),
                        avgActualProt = x.Average(y => y.ActualProtein),
                        avgActualCarb = x.Average(y => y.ActualCarbohydrate)
                    })
                    .ToList();

            var plannedFatCalories = q.Count > 0 ? ((q.First().avgPlannedFat * 9)) : 0;
            var plannedProtCalories = q.Count > 0 ? ((q.First().avgPlannedProt * 4)) : 0;
            var plannedCarbCalories = q.Count > 0 ? ((q.First().avgPlannedCarb * 4)) : 0;
            var actualFatCalories = q.Count > 0 ? ((q.First().avgActualFat * 9)) : 0;
            var actualProtCalories = q.Count > 0 ? ((q.First().avgActualProt * 4)) : 0;
            var actualCarbCalories = q.Count > 0 ? ((q.First().avgActualCarb * 4)) : 0;


            var totalPlannedCalories = (plannedFatCalories + plannedProtCalories + plannedCarbCalories);
            var totalActualCalories = (actualFatCalories + actualProtCalories + actualCarbCalories);

            //TODO: Move this to the repository, add string for current/past/future
            List<CurrentWeekMacroGauge> model = new List<CurrentWeekMacroGauge>()
            {
                new CurrentWeekMacroGauge
                {
                    macro = "Actual Fat", value = Convert.ToDouble(actualFatCalories / totalActualCalories), active = true
                },
                new CurrentWeekMacroGauge
                {
                    macro = "Planned Fat", value = Convert.ToDouble(plannedFatCalories / totalPlannedCalories), active = true
                },
                new CurrentWeekMacroGauge
                {
                    macro = "Actual Protein", value = Convert.ToDouble(actualProtCalories / totalActualCalories), active = true
                },
                new CurrentWeekMacroGauge
                {
                    macro = "Planned Protein", value = Convert.ToDouble(plannedProtCalories / totalPlannedCalories), active = true
                },
                new CurrentWeekMacroGauge
                {
                    macro = "Actual Carbs", value = Convert.ToDouble(actualCarbCalories / totalActualCalories), active = true
                },
                new CurrentWeekMacroGauge
                {
                    macro = "Planned Carbs", value = Convert.ToDouble(plannedCarbCalories / totalPlannedCalories), active = true
                }


            };


            return PartialView("_clientCurrentWeekBarGauge", model);


        }

        public PartialViewResult ClientProgramGrid(int userId)
        {
            var currentWeekNumber = dateRepository.getCurrentWeekNumber(DateTime.Now);
            var userProgress = userProgramRepository.GetActive.Where(x => x.ProgramUserId == userId).Include(y => y.DailyProgress).SelectMany(z => z.DailyProgress);

            var q = (userProgress
                .OrderBy(u => u.DateId)
                )
                .Where(x => x.Dates.WeekOfYear <= currentWeekNumber + 1)
                .Select(x => new
                {
                    x.Dates.Date,
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
                .Select(y => new UserProgramProgress()
                {
                    Date = y.Date,
                    PlannedWeight = y.PlannedWeight,
                    ActualWeight = y.ActualWeight,
                    PlannedFat = y.PlannedFat,
                    ActualFat = y.ActualFat,
                    PlannedProtein = y.PlannedProtein,
                    ActualProtein = y.ActualProtein,
                    PlannedCarbohydrates = y.PlannedCarbohydrate,
                    ActualCarbohydrates = y.ActualCarbohydrate

                });

            return PartialView("_clientProgramGrid", q);
        }
    }
}