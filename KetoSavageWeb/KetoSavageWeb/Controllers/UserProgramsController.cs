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


        public UserProgramsController(ProgramRepository pr, RoleRepository rr, UserProgramRepository up)
        {
            this.program = pr;
            this.roleRepository = rr;
            this.userProgramRepository = up;
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
                    CoachName = x.UserPrograms.Select(y => y.CoachUser.FirstName).FirstOrDefault(),
                    Notes = x.UserPrograms.Select(y => y.Notes).FirstOrDefault(),
                    UserId = x.UserId,
                    ProgramId = x.UserPrograms.Select(y => y.MasterProgramId).FirstOrDefault()
                    
                }
                ));
            ViewBag.ProgramList = new SelectList(program.GetActive, "Id", "programDescription");
            ViewBag.CoachList = new SelectList(coachList, "Id", "UserName");
            var model = items.ToList();
            return PartialView("_userGridViewPartial", model);
        }

        public ActionResult userProgramGridAdd(UserProgramViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUserProgram = new UserPrograms
                {
                    ProgramType = model.ProgramType,
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

        public ActionResult userProgramGridEdit(UserProgramViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newOrModify = userProgramRepository.Find(model.Id);
                if (newOrModify == null)
                {
                    newOrModify = new UserPrograms()
                    {
                        ProgramType = model.ProgramType,
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
                    newOrModify.ProgramType = model.ProgramType;
                    newOrModify.StartDate = model.currentProgramStartDate;
                    newOrModify.EndDate = model.currentProgramEndDate;
                    newOrModify.RenewalDate = model.currentProgramRenewalDate;
                    newOrModify.MasterProgramId = model.ProgramId;
                    newOrModify.LastModified = DateTime.Now;
                    newOrModify.LastModifiedBy = CurrentUser.UserName;

                    userProgramRepository.Update(newOrModify);
                }
                
                return RedirectToAction("Index");
                
            }
            return RedirectToAction("Index");
        }

    }
}