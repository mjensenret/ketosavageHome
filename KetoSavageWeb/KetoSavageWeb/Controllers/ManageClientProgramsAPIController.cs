using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using KetoSavageWeb.Controllers.Abstract;
using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using KetoSavageWeb.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Mvc;

namespace KetoSavageWeb.Controllers
{
    public class ManageClientProgramsAPIController : KSBaseAPIController
    {
        private ProgramRepository program;
        private RoleRepository roleRepository;
        private UserProgramRepository userProgramRepository;
        private IDateRepository dateRepository;

        KSDataContext _context = new KSDataContext();

        public ManageClientProgramsAPIController(UserProgramRepository upr, ProgramRepository pr, IDateRepository dr)
        {
            dateRepository = dr;
            program = pr;
            userProgramRepository = upr;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetClientList(DataSourceLoadOptions loadOptions)
        {
            var coachList = UserManager.Users.Where(x => x.IsActive == true).Where(x => x.Roles.Select(y => y.Role.Name).Contains("Coach")).ToList();
            var userQuery = UserManager.Users.Where(x => x.IsActive == true).Include(x => x.Roles).Include(y => y.UserPrograms);
            userQuery = userQuery.Where(x => x.Roles.Select(y => y.Role.Name).Contains("Client")).Include(z => z.UserPrograms.Select(p => p.DailyProgress));

            var items = (userQuery
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Select(x => new
                {
                    UserId = x.Id,
                    ProgramId = x.UserPrograms.Where(z => z.IsDeleted == false && z.IsActive == true).Select(y => y.Id).FirstOrDefault(),
                    x.FirstName,
                    x.LastName,
                    x.Roles,
                    x.UserName,
                    x.Email,
                    UserPrograms = x.UserPrograms.Where(z => z.IsDeleted == false && z.EndDate >= DateTime.Now && z.IsActive == true)
                })
                .ToList()
                .Select(x => new UserProgramViewModel()
                {
                    Id = x.ProgramId,
                    UserId = x.UserId,
                    UserName = x.UserName,
                    FullName = string.Join(" ", x.FirstName, x.LastName),
                    ProgramName = x.UserPrograms.Select(y => y.MasterProgram.Description).FirstOrDefault(),
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
                    CurrentWeight = getCurrentWeight(x.UserId).HasValue ? getCurrentWeight(x.UserId).Value : x.UserPrograms.Select(y => y.StartWeight).FirstOrDefault()
                }
                ));

            //TODO: Web API for this
            //ViewBag.ProgramList = new SelectList(program.GetActive, "Id", "Description");
            //ViewBag.CoachList = new SelectList(coachList, "Id", "UserName");
            var model = items.ToList();

            return Request.CreateResponse(DataSourceLoader.Load(model, loadOptions));
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetClientPrograms(DataSourceLoadOptions loadOptions)
        {
            
            var userQuery = UserManager.Users.Where(x => x.IsActive == true).Include(x => x.Roles).Include(y => y.UserPrograms);
            userQuery = userQuery.Where(x => x.Roles.Select(y => y.Role.Name).Contains("Client")).Include(z => z.UserPrograms);
            //userQuery = userQuery.Where(x => x.UserPrograms.Select(y => y.IsDeleted == false).FirstOrDefault());

            var items = (userQuery
                .OrderBy(x => x.UserName)
                .Select(x => new
                {
                    UserId = x.Id,
                    ProgramId = x.UserPrograms.Where(z => z.IsDeleted == false && z.IsActive == true).Select(y => y.Id).FirstOrDefault(),
                    x.FirstName,
                    x.LastName,
                    x.Roles,
                    x.UserName,
                    x.Email,
                    UserPrograms = x.UserPrograms.Where(z => z.IsDeleted == false && z.IsActive == true)
                })
                .ToList()
                .Select(x => new UserProgramViewModel()
                {
                    Id = x.ProgramId,
                    UserId = x.UserId,
                    UserName = x.UserName,
                    FullName = string.Join(" ", x.FirstName, x.LastName),
                    ProgramName = x.UserPrograms.Select(y => y.MasterProgram.Description).FirstOrDefault(),
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
                    CurrentWeight = getCurrentWeight(x.UserId).HasValue ? getCurrentWeight(x.UserId).Value : 0.00,
                    IsActive = x.UserPrograms.Select(y => y.IsActive).FirstOrDefault()
                }
                ));
            //ViewBag.ProgramList = new SelectList(program.GetActive, "Id", "Description");
            //ViewBag.CoachList = new SelectList(coachList, "Id", "UserName");

            var model = items.ToList();
            return Request.CreateResponse(DataSourceLoader.Load(model, loadOptions));
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage updateProgram(FormDataCollection form)
        {
            var key = Convert.ToInt32(form.Get("key"));
            var values = form.Get("values");
            var model = new UserProgramViewModel();
            JsonConvert.PopulateObject(values, model);

            if (key == 0)
            {


                var activeProgram = userProgramRepository.GetActive.Where(x => x.ProgramUser.UserName == model.UserName).FirstOrDefault();

                if (activeProgram == null)
                {
                    var userQuery = UserManager.Users.Where(x => x.UserName == model.UserName).First();
                    //var masterProgram = program.GetActive.Where(x => x.Description == model.ProgramName).First();
                    var masterProgram = _context.Programs.Where(x => x.Description == model.ProgramName).First();
                    var currentUserName = RequestContext.Principal.Identity.Name;
                    
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

            }
            else
            {
                var userProgram = userProgramRepository.Find(key);

                bool updStartDate = model.currentProgramStartDate.HasValue;
                bool updEndDate = model.currentProgramEndDate.HasValue;
                bool updStartWeight = (model.StartWeight > 0);
                bool updEndWeight = (model.GoalWeight > 0);


                if (model.ProgramName != null)
                    userProgram.ProgramType = model.ProgramName;
                if (model.currentProgramStartDate.HasValue)
                    userProgram.StartDate = model.currentProgramStartDate;
                if (model.currentProgramRenewalDate.HasValue)
                    userProgram.RenewalDate = model.currentProgramRenewalDate;
                if (model.currentProgramEndDate.HasValue)
                    userProgram.EndDate = model.currentProgramEndDate;
                if (model.StartWeight > 0)
                    userProgram.StartWeight = model.StartWeight;
                if (model.GoalWeight > 0)
                    userProgram.GoalWeight = model.GoalWeight;
                if (model.MasterProgramId > 0)
                    userProgram.MasterProgramId = model.MasterProgramId;

                if (model.Notes != null)
                    userProgram.Notes = model.Notes;
                if (model.CoachId > 0)
                    userProgram.CoachUserId = model.CoachId;
                userProgram.IsActive = model.IsActive;
                userProgram.LastModified = DateTime.Now;
                userProgram.LastModifiedBy = RequestContext.Principal.Identity.Name;

                userProgramRepository.Update(userProgram);

                var userDailyProgress = _context.DailyProgress.Where(x => x.UserProgramId == userProgram.Id).OrderBy(x => x.DateId);

                //Add or remove daily progress entries affected by the start date
                if (updStartDate)
                {
                    if (userDailyProgress.Select(x => x.Dates.Date).First() < userProgram.StartDate.Value)
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
                                CreatedBy = "GetCurrentName",
                                LastModified = DateTime.Now,
                                LastModifiedBy = "GetCurrentName"
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
                                CreatedBy = "GetCurrentName",
                                LastModified = DateTime.Now,
                                LastModifiedBy = "GetCurrentName"
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
            }


            return Request.CreateResponse(HttpStatusCode.OK);

        }

        //Programs
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getProgramList(DataSourceLoadOptions loadOptions)
        {
            var programs = _context.Programs.Where(x => x.IsActive);
            return Request.CreateResponse(DataSourceLoader.Load(programs, loadOptions));
        }

        //Coach List
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getCoachList(DataSourceLoadOptions loadOptions)
        {
            var coachList = UserManager.Users.Where(x => x.IsActive == true).Where(x => x.Roles.Select(y => y.Role.Name).Contains("Coach")).ToList();
            return Request.CreateResponse(DataSourceLoader.Load(coachList, loadOptions));
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
            double lbsPerDay = Math.Round((endWeight - beginningWeight) / days, 2);

            foreach (var day in dp)
            {
                day.PlannedWeight = beginningWeight;
                beginningWeight = Math.Round(beginningWeight + lbsPerDay, 2);
            }

            _context.SaveChanges();

        }
    }
}
