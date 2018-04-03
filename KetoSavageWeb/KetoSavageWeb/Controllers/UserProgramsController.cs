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
            
            var coachList = UserManager.Users.Where(x => x.IsActive == true).Where(x => x.Roles.Select(y => y.Role.Name).Contains("Coach")).ToList();
            var userQuery = UserManager.Users.Where(x => x.IsActive == true).Include(x => x.Roles).Include(y => y.UserPrograms);
            userQuery = userQuery.Where(x => x.Roles.Select(y => y.Role.Name).Contains("Client"));

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
                    UserId = x.UserId,
                    ProgramId = x.UserPrograms.Select(y => y.MasterProgramId).FirstOrDefault(),
                    CoachId = x.UserPrograms.Select(y => y.CoachUserId).FirstOrDefault(),
                    ProgramUserId = x.UserPrograms.Select(y => y.ProgramUserId).FirstOrDefault(),
                    StartWeight = x.UserPrograms.Select(y => y.StartWeight).FirstOrDefault(),
                    GoalWeight = x.UserPrograms.Select(y => y.GoalWeight).FirstOrDefault()
                    
                    
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
                    MasterProgramId = model.ProgramId,
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
                        MasterProgramId = model.ProgramId,
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
                    newOrModify.MasterProgramId = model.ProgramId;
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
            
            var coachList = UserManager.Users.Where(x => x.IsActive == true).Where(x => x.Roles.Select(y => y.Role.Name).Contains("Coach")).ToList();

            var userProgram = userProgramRepository.GetActive.Where(x => x.ProgramUserId == userId).FirstOrDefault();
            var user = await UserManager.FindByIdAsync(userId);

            var model = new UserProgramViewModel();
            if (userProgram == null)
            {
                model.UserName = user.UserName;
                model.ProgramId = 1;
                model.ProgramUserId = user.Id;
                model.FullName = string.Join(" ", user.FirstName, user.LastName);
                model.currentProgramStartDate = DateTime.Now.Date;
                model.currentProgramRenewalDate = DateTime.Now.Date.AddDays(30);
                model.currentProgramEndDate = DateTime.Now.Date.AddMonths(3);
                model.CoachId = 2;
                model.UserType = "Client";
                model.IsNew = true;
                ViewBag.IsNew = true;
            }
            else
            {
                model.ProgramUserId = userProgram.ProgramUserId;
                model.UserName = userProgram.ProgramUser.UserName;
                model.FullName = string.Join(" ", user.FirstName, user.LastName);
                model.CoachId = userProgram.CoachUserId;
                model.ProgramId = userProgram.MasterProgramId;
                model.Notes = userProgram.Notes;
                model.ProgramName = userProgram.MasterProgram.Name;
                model.currentProgramStartDate = userProgram.StartDate;
                model.currentProgramRenewalDate = userProgram.RenewalDate;
                model.currentProgramEndDate = userProgram.EndDate;
                model.StartWeight = userProgram.StartWeight;
                model.GoalWeight = userProgram.GoalWeight;
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
                if (model.IsNew)
                {
                    UserPrograms newProgram = new UserPrograms()
                    {
                        MasterProgramId = model.ProgramId,
                        ProgramType = model.ProgramType,
                        ProgramUserId = model.ProgramUserId,
                        StartDate = model.currentProgramStartDate,
                        EndDate = model.currentProgramEndDate,
                        RenewalDate = model.currentProgramRenewalDate,
                        StartWeight = model.StartWeight,
                        GoalWeight = model.GoalWeight,
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
                    var updProgram = userProgramRepository.GetActive.Where(x => x.ProgramUserId == model.ProgramUserId).FirstOrDefault();
                    updProgram.MasterProgramId = model.ProgramId;
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

                    return RedirectToAction("ShowProgramDetails", new { @_userId = model.UserId });
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
                    up.ProgramUserId,
                    up.ProgramUser.FirstName,
                    up.ProgramUser.LastName,

                })
                .ToList()
                .Select(x => new UserProgramViewModel()
                {
                    ProgramUserId = x.ProgramUserId,
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
    }
}