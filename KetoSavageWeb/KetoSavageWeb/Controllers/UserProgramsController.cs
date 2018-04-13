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
using DevExpress.Web.Mvc;

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
            
            var coachList = UserManager.Users.Where(x => x.IsActive == true).Where(x => x.Roles.Select(y => y.Role.Name).Contains("Coach")).ToList();
            var userQuery = UserManager.Users.Where(x => x.IsActive == true).Include(x => x.Roles).Include(y => y.UserPrograms);
            userQuery = userQuery.Where(x => x.Roles.Select(y => y.Role.Name).Contains("Client")).Include(z => z.UserPrograms.Select(p => p.DailyProgress));


            var items = (userQuery
                .OrderBy(x => x.UserName)
                .Select(x => new
                {
                    UserId = x.Id,
                    ProgramId = x.UserPrograms.Select(y => y.Id),
                    x.FirstName,
                    x.LastName,
                    x.Roles,
                    x.UserName,
                    x.Email,
                    x.UserPrograms
                })
                .ToList()
                .Select(x => new UserProgramViewModel()
                {
                    Id = x.UserPrograms.Select(y => y.Id).FirstOrDefault(),
                    UserId = x.UserId,
                    UserName = x.UserName,
                    FullName = string.Join(" ", x.FirstName, x.LastName),
                    ProgramName = x.UserPrograms.Select(y => y.MasterProgram.programDescription).FirstOrDefault(),
                    UserType = x.Roles.Select(y => y.Role.Name).FirstOrDefault(),
                    currentProgramStartDate = x.UserPrograms.Select(y => y.StartDate).FirstOrDefault(),
                    currentProgramRenewalDate = x.UserPrograms.Select(y => y.RenewalDate).FirstOrDefault(),
                    currentProgramEndDate = x.UserPrograms.Select(y => y.EndDate).FirstOrDefault(),
                    LastModified = x.UserPrograms.Select(y => y.LastModified).FirstOrDefault(),
                    CoachName = x.UserPrograms.Select(y => y.CoachUser.FirstName).FirstOrDefault(),
                    Notes = x.UserPrograms.Select(y => y.Notes).FirstOrDefault(),
                    MasterProgramId = x.UserPrograms.Select(y => y.MasterProgramId).FirstOrDefault(),
                    CoachId = x.UserPrograms.Select(y => y.CoachUserId).FirstOrDefault(),
                    StartWeight = x.UserPrograms.Select(y => y.StartWeight).FirstOrDefault(),
                    GoalWeight = x.UserPrograms.Select(y => y.GoalWeight).FirstOrDefault(),
                    CurrentWeight = getCurrentWeight(x.UserId).HasValue ? getCurrentWeight(x.UserId).Value : 0.00
                }
                ));
            ViewBag.ProgramList = new SelectList(program.GetActive, "Id", "programDescription");
            ViewBag.CoachList = new SelectList(coachList, "Id", "UserName");
            var model = items.ToList();
            return PartialView("_userGridViewPartial", model);
        }

        public ActionResult userProgramAdd(UserProgramViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUserProgram = new UserPrograms
                {
                    ProgramType = model.UserType,
                    StartDate = model.currentProgramStartDate,
                    EndDate = model.currentProgramEndDate,
                    RenewalDate = model.currentProgramRenewalDate,
                    ProgramUserId = model.UserId,
                    MasterProgramId = model.MasterProgramId,
                    CoachUserId = model.CoachId,
                    Created = DateTime.Now,
                    CreatedBy = CurrentUser.UserName,
                    LastModified = DateTime.Now,
                    LastModifiedBy = CurrentUser.UserName,
                    IsActive = true,
                    IsDeleted = false
                };

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public ActionResult userProgramEdit(UserProgramViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newOrModify = userProgramRepository.Find(model.Id);
                if (newOrModify == null)
                {
                    newOrModify = new UserPrograms()
                    {
                        ProgramType = model.UserType,
                        StartDate = model.currentProgramStartDate,
                        RenewalDate = model.currentProgramRenewalDate,
                        EndDate = model.currentProgramEndDate,
                        ProgramUserId = model.UserId,
                        MasterProgramId = model.MasterProgramId,
                        CoachUserId = model.CoachId,
                        Notes = model.Notes,
                        LastModified = DateTime.Now,
                        LastModifiedBy = CurrentUser.UserName,
                        Created = DateTime.Now,
                        CreatedBy = CurrentUser.UserName
                    };
                    userProgramRepository.Create(newOrModify);
                }
                else
                {
                    newOrModify.ProgramUserId = model.UserId;
                    newOrModify.ProgramType = model.ProgramType;
                    newOrModify.StartDate = model.currentProgramStartDate;
                    newOrModify.EndDate = model.currentProgramEndDate;
                    newOrModify.RenewalDate = model.currentProgramRenewalDate;
                    newOrModify.MasterProgramId = model.MasterProgramId;
                    newOrModify.CoachUserId = model.CoachId;
                    newOrModify.Notes = model.Notes;
                    newOrModify.LastModified = DateTime.Now;
                    newOrModify.LastModifiedBy = CurrentUser.UserName;

                    userProgramRepository.Update(newOrModify);
                }
                
                return RedirectToAction("Index");
                
            }
            return RedirectToAction("Index");
        }
        
        public async Task<ActionResult> ShowProgramDetails(string _userId)
        {
            var userId = Convert.ToInt32(_userId);
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
                        .OrderByDescending(t => t.DateId)
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
                model.IsNew = false;
                ViewBag.IsNew = false;
            };

            ViewBag.ProgramList = new SelectList(program.GetActive, "Id", "programDescription");
            ViewBag.CoachList = new SelectList(coachList, "Id", "UserName");

            return View("UserProgramDetails", model);
        }

        [HttpPost]
        public ActionResult updateProgram(UserProgramViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsNew || model.Id == 0)
                {
                    UserPrograms newProgram = new UserPrograms()
                    {
                        ProgramType = model.ProgramType,
                        ProgramUserId = model.UserId,
                        StartDate = model.currentProgramStartDate,
                        EndDate = model.currentProgramEndDate,
                        RenewalDate = model.currentProgramRenewalDate,
                        StartWeight = model.StartWeight,
                        GoalWeight = model.GoalWeight,
                        MasterProgramId = model.MasterProgramId,
                        Notes = model.Notes,
                        CoachUserId = model.CoachId,
                        Created = DateTime.Now,
                        CreatedBy = CurrentUser.UserName,
                        LastModified = DateTime.Now,
                        LastModifiedBy = CurrentUser.UserName
                    };
                    
                    _context.UserPrograms.Add(newProgram);
                    
                    DateTime startDate = Convert.ToDateTime(newProgram.StartDate);
                    var numberOfDays = (newProgram.EndDate.Value.Date - newProgram.StartDate.Value.Date).TotalDays;
                    double lbsPerDay = Math.Round((newProgram.StartWeight - newProgram.GoalWeight) / numberOfDays,2);

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
                            newWeight = Math.Round((newProgram.StartWeight - lbsPerDay),2);
                        }
                        else
                        {
                            dailyProgress.DateId = dateRepository.getDateKey(startDate);
                            dailyProgress.PlannedWeight = newWeight;
                            startDate = startDate.AddDays(1);
                            newWeight = Math.Round((newWeight - lbsPerDay),2);
                        }

                        //dailyProgress.UserProgramId = newProgramId;
                        dailyProgress.Created = DateTime.Now;
                        dailyProgress.CreatedBy = CurrentUser.UserName;
                        dailyProgress.LastModified = DateTime.Now;
                        dailyProgress.LastModifiedBy = CurrentUser.UserName;

                        _context.DailyProgress.Add(dailyProgress);
                        numberOfDays--;
                    }

                    _context.SaveChanges();
                    return RedirectToAction("ShowProgramDetails", new { @_userId = newProgram.ProgramUserId.ToString()});


                }
                else
                {
                    var updProgram = userProgramRepository.GetActive.Where(x => x.Id == model.Id).FirstOrDefault();
                    updProgram.MasterProgramId = model.MasterProgramId;
                    updProgram.StartDate = model.currentProgramStartDate;
                    updProgram.EndDate = model.currentProgramEndDate;
                    updProgram.RenewalDate = model.currentProgramRenewalDate;
                    updProgram.StartWeight = model.StartWeight;
                    updProgram.GoalWeight = model.GoalWeight;
                    updProgram.Notes = model.Notes;
                    updProgram.CoachUserId = model.CoachId;
                    updProgram.LastModified = DateTime.Now;
                    updProgram.LastModifiedBy = CurrentUser.UserName;

                    userProgramRepository.Update(updProgram);

                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("", "Something failed editing the program template!");
                return RedirectToAction("ShowProgramDetails", new { @_userId = model.UserId });
            }
        }
        [HttpPost]
        public PartialViewResult UserProgramDetails(string _userId)
        {
            var userId = Convert.ToInt32(_userId);

            var userProgram = userProgramRepository.GetActive.Where(x => x.ProgramUserId == userId);

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
            userProgramDetails.Where(x => x.Dates.WeekOfYear == currentWeek || x.Dates.WeekOfYear == (currentWeek - 1));


            var q = (userProgramDetails
                .Where(x => x.Dates.WeekOfYear == currentWeek || x.Dates.WeekOfYear == (currentWeek - 1))
                .OrderByDescending(x => x.Dates.Date)
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
                    WeekNum = y.Dates.WeekOfYear,
                    WeekdayName = y.Dates.WeekDayName,
                    PlannedFat = Convert.ToDouble(y.PlannedFat),
                    PlannedProtein = Convert.ToDouble(y.PlannedProtein),
                    PlannedCarbs = Convert.ToDouble(y.PlannedCarbohydrate),
                    IsRefeed = y.IsRefeed
                })
                );

            return PartialView("_dailyMacros", q);
        }

        [HttpPost, ValidateInput(true)]
        public ActionResult BatchUpdateMacros(MVCxGridViewBatchUpdateValues<UpdateMacrosViewModel, int> batchValues)
        {
            foreach (var item in batchValues.Update)
            {
                if (batchValues.IsValid(item))
                    userProgramRepository.UpdateItem(item, batchValues);
                else
                    batchValues.SetErrorText(item, "Correct validation errors");
            }
            

            return RedirectToAction("ShowProgramDetails", new { @_userId = Session["UserId"] });
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
    }


}