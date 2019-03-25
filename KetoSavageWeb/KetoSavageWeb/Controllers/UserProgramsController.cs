using KetoSavageWeb.Interfaces;
using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KetoSavageWeb.ViewModels;

namespace KetoSavageWeb.Controllers
{
    public class UserProgramsController : KSBaseController
    {
        private ProgramRepository program;
        private RoleRepository roleRepository;
        private UserProgramRepository userProgramRepository;
        private DateRepository dateRepository;

        private KSDataContext _context = new KSDataContext();



        public UserProgramsController(ProgramRepository pr, RoleRepository rr, UserProgramRepository up, DateRepository dr)
        {
            this.program = pr;
            this.roleRepository = rr;
            this.userProgramRepository = up;
            this.dateRepository = dr;
        }
        // GET: Programs
        public async Task<ActionResult> Index()
        {
            return View();
        }

        public ActionResult UserProgramList()
        {
            //Session["currentUser"] = User.Identity.Name;
            return PartialView("_dxUserGridViewPartial");
        }

        public ActionResult ManageClientMacros()
        {
            return View();
        }

        public ActionResult ManageClientMacrosList()
        {
             return PartialView("_dxManageMacroDataGrid");
        }

        public ActionResult UpdateUserProgram(int? userId)
        {
            ViewBag.UserId = userId;
            return PartialView("_dxAddUpdateUserProgramForm");
        }
        public ActionResult userProgramAdd(UserProgramViewModel model)
        {
            if (ModelState.IsValid)
            {
                var activeProgram = userProgramRepository.GetActive.Where(x => x.ProgramUser.UserName == model.UserName).FirstOrDefault();

                if (activeProgram == null) {
                    var userQuery = UserManager.Users.Where(x => x.UserName == model.UserName).First();
                    var masterProgram = program.GetActive.Where(x => x.Id == model.MasterProgramId).First();
                    var currentUserName = CurrentUser.UserName;
                    UserPrograms newProgram = new UserPrograms()
                    {
                        ProgramType = masterProgram.Name,
                        ProgramUserId = userQuery.Id,
                        StartDate = model.currentProgramStartDate,
                        EndDate = model.currentProgramEndDate,
                        RenewalDate = model.currentProgramRenewalDate,
                        StartWeight = model.StartWeight,
                        GoalWeight = model.GoalWeight,
                        MasterProgramId = masterProgram.Id,
                        IsActive = true,
                        Notes = model.Notes,
                        CoachUserId = model.CoachId,
                        Created = DateTime.Now,
                        CreatedBy = currentUserName,
                        LastModified = DateTime.Now,
                        LastModifiedBy = currentUserName
                    };

                    _context.UserPrograms.Add(newProgram);

                    DateTime startDate = Convert.ToDateTime(newProgram.StartDate);
                    var numberOfDays = (newProgram.EndDate.Value.Date - newProgram.StartDate.Value.Date).TotalDays;
                    double lbsPerDay = Math.Round((newProgram.StartWeight - newProgram.GoalWeight) / numberOfDays, 2);

                    double newWeight = 0;

                    //var dailyProgress = new DailyProgress();
                    while (numberOfDays > 0)
                    {
                        DailyProgress dailyProgress = new DailyProgress();
                        dailyProgress.UserProgram = newProgram;
                        if (startDate == newProgram.StartDate)
                        {

                            dailyProgress.DateId = dateRepository.getDateKey(startDate);
                            dailyProgress.PlannedWeight = newProgram.StartWeight;
                            startDate = startDate.AddDays(1);
                            newWeight = Math.Round((newProgram.StartWeight - lbsPerDay), 2);
                        }
                        else
                        {
                            dailyProgress.DateId = dateRepository.getDateKey(startDate);
                            dailyProgress.PlannedWeight = newWeight;
                            startDate = startDate.AddDays(1);
                            newWeight = Math.Round((newWeight - lbsPerDay), 2);
                        }

                        //dailyProgress.UserProgramId = newProgramId;
                        dailyProgress.Created = DateTime.Now;
                        dailyProgress.CreatedBy = currentUserName;
                        dailyProgress.LastModified = DateTime.Now;
                        dailyProgress.LastModifiedBy = currentUserName;

                        _context.DailyProgress.Add(dailyProgress);
                        numberOfDays--;
                    }

                    _context.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult userProgramUpdate(UserProgramViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var userProgram = userProgramRepository.Get.Where(x => x.Id == model.Id).Include(y => y.DailyProgress).First();
                var masterProgram = program.GetActive.Where(x => x.Id == model.MasterProgramId).First();
                var currentUserName = CurrentUser.UserName;
                bool updStartDate = (userProgram.StartDate != model.currentProgramStartDate);
                bool updEndDate = (userProgram.EndDate != model.currentProgramEndDate);
                bool updStartWeight = (userProgram.StartWeight != model.StartWeight);
                bool updEndWeight = (userProgram.GoalWeight != model.GoalWeight);

                userProgram.ProgramType = masterProgram.Name;
                userProgram.StartDate = model.currentProgramStartDate;
                userProgram.EndDate = model.currentProgramEndDate;
                userProgram.RenewalDate = model.currentProgramRenewalDate;
                userProgram.StartWeight = model.StartWeight;
                userProgram.GoalWeight = model.GoalWeight;
                userProgram.MasterProgramId = masterProgram.Id;
                userProgram.IsActive = model.IsActive;
                userProgram.Notes = model.Notes;
                userProgram.CoachUserId = model.CoachId;
                userProgram.LastModified = DateTime.Now;
                userProgram.LastModifiedBy = currentUserName;

                userProgramRepository.Update(userProgram);

                var userDailyProgress = _context.DailyProgress.Where(x => x.UserProgramId == userProgram.Id).OrderBy(x => x.DateId);
                

                //Add or remove daily progress entries affected by the start date
                if (updStartDate)
                {
                    if(userDailyProgress.Select(x => x.Dates.Date).First() < userProgram.StartDate.Value)
                    {
                        var oldDays = userDailyProgress
                            .Where(x => x.Dates.Date < userProgram.StartDate.Value);

                        foreach (var day in oldDays)
                        {
                            _context.DailyProgress.Remove(day);
                        }
                        _context.SaveChanges();
                    }
                    else
                    {
                        var addDays = (userDailyProgress.Select(x => x.Dates.Date).First() - userProgram.StartDate.Value).TotalDays;
                        var startDate = userProgram.StartDate.Value;

                        while (addDays > 0)
                        {

                            DailyProgress addDailyProgress = new DailyProgress
                            {
                                UserProgram = userDailyProgress.Select(x => x.UserProgram).First(),
                                DateId = dateRepository.getDateKey(startDate),
                                PlannedWeight = 0,
                                Created = DateTime.Now,
                                CreatedBy = currentUserName,
                                LastModified = DateTime.Now,
                                LastModifiedBy = currentUserName
                            };
                            _context.DailyProgress.Add(addDailyProgress);
                            startDate = startDate.AddDays(1);
                            addDays--;
                        }
                        
                        _context.SaveChanges();
                    }
                }

                if (updEndDate)
                {
                    var lastDailyProgress = userDailyProgress.OrderByDescending(x => x.DateId).Select(x => x.Dates.Date).First();
                    if (lastDailyProgress < userProgram.EndDate.Value)
                    {
                        var addDays = (userProgram.EndDate.Value - lastDailyProgress).TotalDays;
                        DateTime endDate;
                        if (userDailyProgress.Count() < 1)
                        {
                            endDate = lastDailyProgress;
                        }
                        else
                        {
                            endDate = lastDailyProgress.AddDays(1);
                        }

                        do
                        {
                            DailyProgress addDailyProgress = new DailyProgress
                            {
                                UserProgramId = userProgram.Id,
                                DateId = dateRepository.getDateKey(endDate),
                                PlannedWeight = 0,
                                Created = DateTime.Now,
                                CreatedBy = currentUserName,
                                LastModified = DateTime.Now,
                                LastModifiedBy = currentUserName
                            };
                            _context.DailyProgress.Add(addDailyProgress);
                            endDate = endDate.AddDays(1);
                            addDays--;
                        }
                        while (addDays > 0);

                        _context.SaveChanges();
                    }
                    else
                    {
                        var oldDays = userDailyProgress
                                .Where(x => x.Dates.Date > userProgram.EndDate.Value);

                        foreach (var day in oldDays)
                        {
                            _context.DailyProgress.Remove(day);
                        }
                        _context.SaveChanges();
                    }
                }

                if (userProgram.IsActive && !userProgram.IsDeleted && (updEndDate || updStartDate || updStartWeight || updEndWeight))
                    updateWeightPerDay(userDailyProgress, userProgram.StartDate.Value, userProgram.EndDate.Value, userProgram.StartWeight, userProgram.GoalWeight);

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Something failed editing the program template!");
                return RedirectToAction("ShowProgramDetails", new { @_userId = model.UserId });
            }
        }

        [HttpPost]
        public ActionResult userProgramDelete(UserProgramViewModel model)
        {

            var userProgram = userProgramRepository.Get.Where(x => x.Id == model.Id).Include(y => y.DailyProgress).First();

            userProgram.IsActive = false;
            userProgram.IsDeleted = true;

            userProgramRepository.Update(userProgram);

            var userDailyProgress = _context.DailyProgress.Where(x => x.UserProgramId == userProgram.Id);

            foreach (var dp in userDailyProgress)
            {
                dp.IsActive = false;
                dp.IsDeleted = true;
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        
        public async Task<ActionResult> ShowProgramDetails(string uid)
        {
            var userId = Convert.ToInt32(uid);
            Session["UserId"] = userId;
            
            var coachList = UserManager.Users.Where(x => x.IsActive == true).Where(x => x.Roles.Select(y => y.Role.Name).Contains("Coach")).ToList();

            double? currentWeight = null;

            var userProgram = userProgramRepository.GetActive.Where(x => x.ProgramUser.Id == userId).Include(d => d.DailyProgress).FirstOrDefault();
            if (userProgram != null)
            {
                if (userProgram.DailyProgress != null)
                {
                    currentWeight = userProgram.DailyProgress
                        .Where(w => w.ActualWeight != null)
                        .OrderBy(t => t.DateId)
                        .Select(x => x.ActualWeight)
                        .FirstOrDefault();
                }
            }

            var user = await UserManager.FindByIdAsync(userId);
            //TODO: update coach to choose the default coach.
            //TODO: update default program setting.
            var model = new UserProgramViewModel();
            if (userProgram == null)
            {
                model.UserId = user.Id;
                model.UserName = user.UserName;
                model.MasterProgramId = 1;
                model.FullName = string.Join(" ", user.FirstName, user.LastName);
                model.currentProgramStartDate = DateTime.Now.Date;
                model.currentProgramRenewalDate = DateTime.Now.Date.AddDays(30);
                model.currentProgramEndDate = DateTime.Now.Date.AddMonths(3);
                model.CoachId = 2;
                model.StartWeight = 0;
                model.GoalWeight = 0;
                model.UserType = "Client";
                model.IsNew = true;
                ViewBag.IsNew = true;
            }
            else
            {
                model.Id = userProgram.Id;
                model.MasterProgramId = userProgram.MasterProgramId;
                model.UserId = userProgram.ProgramUser.Id;
                model.UserName = userProgram.ProgramUser.UserName;
                model.FullName = string.Join(" ", user.FirstName, user.LastName);
                model.CoachId = userProgram.CoachUserId;
                model.Notes = userProgram.Notes;
                model.ProgramName = userProgram.MasterProgram.Name;
                model.currentProgramStartDate = userProgram.StartDate;
                model.currentProgramRenewalDate = userProgram.RenewalDate;
                model.currentProgramEndDate = userProgram.EndDate;
                model.StartWeight = userProgram.StartWeight;
                model.GoalWeight = userProgram.GoalWeight;
                model.CurrentWeight = getCurrentWeight(userProgram.ProgramUserId).HasValue ? getCurrentWeight(userProgram.ProgramUserId).Value : userProgram.StartWeight;
                model.UserType = userProgram.ProgramType;
                model.dailyProgress = userProgram.DailyProgress;
                model.IsNew = false;
                ViewBag.IsNew = false;
            };

            ViewBag.ProgramList = new SelectList(program.GetActive, "Id", "Description");
            ViewBag.CoachList = new SelectList(coachList, "Id", "UserName");
            ViewBag.ClientName = model.FullName;

            return View("UserProgramDetails", model);
        }


        [HttpPost]
        public PartialViewResult UserProgramDetails(string _userId)
        {
            var userId = Convert.ToInt32(_userId);


            var userProgram = userProgramRepository.GetActive.Where(x => x.ProgramUserId == userId);
            ViewBag.ClientName = (userProgram.Select(x => x.ProgramUser.FirstName) + " " +
                                  userProgram.Select(x => x.ProgramUser.LastName));

            var item = (userProgram
                .Select(up => new
                {
                    up.Id,
                    UserId = up.ProgramUser.Id,
                    up.ProgramUser.FirstName,
                    up.ProgramUser.LastName,

                })
                .ToList()
                .Select(x => new UserProgramViewModel()
                {
                    UserId = x.UserId,
                    FullName = string.Join(" ", x.FirstName, x.LastName),
                    Notes = "Some information about their program progress and you can update macros here?",
                    ProgramName = "ProgramName"

                }
                ));

            var model = item;

            return PartialView("_userProgramDetails", model);
        }

        public PartialViewResult MacroPieChart()
        {
            List<MacroPieChart> model = new List<MacroPieChart>()
            {
                new MacroPieChart
                {
                    macro = "Fat", value = 165.00*9 / 1925
                },
                new MacroPieChart
                {
                    macro = "Protein", value = 100.00*4 / 1925
                },
                new MacroPieChart
                {
                    macro = "Carbs", value = 10.00*4 / 1925
                    //macro = "Carbs", value = Convert.ToDouble((10*4) / 1925)
                }

            };
            return PartialView("_macroPieChart", model);
        }

        [HttpPost]
        public PartialViewResult onMacroChange(string fat, string protein, string carbs)
        {
            var fatCalories = Convert.ToDouble(fat) * 9;
            var proteinCalories = Convert.ToDouble(protein) * 4;
            var carbCalories = Convert.ToDouble(carbs) * 4;
            var totalCalories = fatCalories + proteinCalories + carbCalories;

            List<MacroPieChart> model = new List<MacroPieChart>()
            {
                new MacroPieChart
                {
                    macro = "Fat", value = fatCalories / totalCalories
                },
                new MacroPieChart
                {
                    macro = "Protein", value = (proteinCalories > 0) ? proteinCalories / totalCalories : 0
                },
                new MacroPieChart
                {
                    macro = "Carbs", value = (carbCalories > 0) ? carbCalories / totalCalories : 0
                }
            };

            return PartialView("_macroPieChart", model);
        }

        public PartialViewResult pastPerformance(int _userId)
        {
            var currentDate = DateTime.Now;
            ViewData["userId"] = _userId;
            var programDetails = userProgramRepository.GetPastProgressByUser(_userId, currentDate);
            

            if (programDetails.Count() > 0)
            {
                var q = (programDetails
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
                        ActualProtein = y.ActualProtein,

                    })
                    );

                return PartialView("_pastPerformance", q);
                //TODO: update progress
            }
            else
            {
                //TODO: Add progress
                return PartialView("_pastPerformance");
            }
        }
        public PartialViewResult NewMacrosGridview()
        {
            var userId = Session["UserId"];
            var currentWeek = dateRepository.GetWeekNum(DateTime.Now.Date);
            var userProgramDetails = userProgramRepository.GetDailyProgressByUser(Convert.ToInt32(userId));
            //userProgramDetails.Where(x => x.Dates.WeekOfYear == currentWeek || x.Dates.WeekOfYear == (currentWeek - 1));


            var q = (userProgramDetails
                //.Where(x => x.Dates.WeekOfYear == currentWeek || x.Dates.WeekOfYear == (currentWeek - 1))
                .OrderBy(x => x.Dates.Date)
                .Select(x => new
                {
                    x.DateId,
                    x.Dates,
                    x.Id,
                    x.UserProgram.ProgramUserId,
                    x.PlannedFat,
                    x.PlannedProtein,
                    x.PlannedCarbohydrate,
                    x.PlannedWeight,
                    x.IsRefeed
                })
                .ToList()
                .Select(y => new UpdateMacrosViewModel()
                {
                    Id = y.Id,
                    DateKey = y.Dates.DateKey,
                    Date = y.Dates.Date,
                    WeekNum = y.Dates.ISOWeekOfYear,
                    WeekdayName = y.Dates.WeekDayName,
                    PlannedFat = Convert.ToDouble(y.PlannedFat),
                    PlannedProtein = Convert.ToDouble(y.PlannedProtein),
                    PlannedCarbs = Convert.ToDouble(y.PlannedCarbohydrate),
                    IsRefeed = y.IsRefeed
                })
                );

            return PartialView("_dailyMacros", q);
        }

        //[HttpPost, ValidateInput(true)]
        //public ActionResult BatchUpdateMacros(MVCxGridViewBatchUpdateValues<UpdateMacrosViewModel, int> batchValues)
        //{
        //    foreach (var item in batchValues.Update)
        //    {
        //        if (batchValues.IsValid(item))
        //            userProgramRepository.UpdateItem(item, batchValues);
        //        else
        //            batchValues.SetErrorText(item, "Correct validation errors");
        //    }
            

        //    return RedirectToAction("ShowProgramDetails", new { @_userId = Session["UserId"] });
        //}

        public ActionResult WeeklyMacroUpdateForm()
        {
            return PartialView("_weeklyMacroUpdateForm");
        }

        //Enter weekly macros
        public ActionResult EnterMacroForm()
        {
            
            DailyMacroUpdate model = new DailyMacroUpdate();
            model.userId = Convert.ToInt32(Session["userId"]);
            model.week = DateTime.Now;
            var weekOfYear = dateRepository.GetWeekNum(model.week);
            var userProgram = userProgramRepository.GetActive.Where(p => p.ProgramUserId == model.userId)
                .Include(d => d.DailyProgress).FirstOrDefault();
            var dailyProgress = userProgram.DailyProgress.Where(d => d.Dates.ISOWeekOfYear == weekOfYear).FirstOrDefault();

            model.Fat = (dailyProgress != null) ? Convert.ToInt32(dailyProgress.PlannedFat) : 0;
            model.Protein = (dailyProgress != null) ? Convert.ToInt32(dailyProgress.PlannedProtein) : 0;
            model.Carbohydrates = (dailyProgress != null) ? Convert.ToInt32(dailyProgress.PlannedCarbohydrate) : 0;

            return PartialView("_dxEnterWeeklyMacroForm",model);
        }


        [HttpPost]
        public ActionResult SetWeeklyMacros(DailyMacroUpdate model)
        {
            var userProgram = userProgramRepository.GetActive.Where(p => p.ProgramUserId == model.userId).Include(d => d.DailyProgress).FirstOrDefault();
            var updWeekNum = dateRepository.GetWeekNum(model.week);
            var dailyProgress = userProgram.DailyProgress.Where(d => d.Dates.ISOWeekOfYear == updWeekNum);

            foreach (var d in dailyProgress)
            {
                d.PlannedFat = model.Fat;
                d.PlannedProtein = model.Protein;
                d.PlannedCarbohydrate = model.Carbohydrates;
                d.LastModified = DateTime.Now;
                d.LastModifiedBy = CurrentUser.UserName;

            }
            userProgram.LastModifiedBy = CurrentUser.UserName;
            userProgram.LastModified = DateTime.Now;

            userProgramRepository.Update(userProgram);

            return RedirectToAction("ShowProgramDetails", new { @uid = Session["UserId"] });
            //return null;
        }


        public double? getCurrentWeight(int userId)
        {
            double? currentWeight = null;
            var userProgram = userProgramRepository.GetActive.Where(x => x.ProgramUserId == userId).Include(y => y.DailyProgress).FirstOrDefault();
            if (userProgram != null)
            { 
                
                if (userProgram.DailyProgress != null)
                {
                    currentWeight = userProgram.DailyProgress
                        .Where(w => w.ActualWeight != null)
                        .OrderByDescending(t => t.DateId)
                        .Select(x => x.ActualWeight)
                        .FirstOrDefault();
                }
            }

            return currentWeight;
        }

        private void updateWeightPerDay(IEnumerable<DailyProgress> userDailyProgress, DateTime startDate, DateTime endDate, double startWeight, double endWeight)
        {
            var dp = userDailyProgress.OrderBy(x => x.DateId);
            double beginningWeight = startWeight;
            var start = startDate;

            if (startDate.Date < DateTime.Now.Date)
            {
                start = DateTime.Now.Date;
                var weightDp = dp.Where(x => x.Dates.Date <= start).Where(y => y.ActualWeight != null).OrderByDescending(z => z.DateId);
                
                beginningWeight = (weightDp.Count() > 0) ? Convert.ToDouble(weightDp.Select(x => x.ActualWeight).First()) : beginningWeight;
                dp = dp.Where(x => x.Dates.Date >= start.Date).OrderBy(y => y.DateId);
            }

            double days = (endDate.Date - start.Date).TotalDays;
            double lbsPerDay = Math.Round((endWeight - beginningWeight)/days, 2);

            foreach (var day in dp)
            {
                day.PlannedWeight = beginningWeight;
                beginningWeight = Math.Round(beginningWeight + lbsPerDay, 2);
            }

            _context.SaveChanges();

        }

        public ActionResult EntryPopup(string buttonName)
        {
            if (buttonName == "Macros")
            {
                return RedirectToAction("EnterMacroForm");
            }
            else
            {
                //return RedirectToAction("EnterMeasurementsForm");
                return null;
            }

        }
    }


}