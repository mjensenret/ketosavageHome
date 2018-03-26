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
            var weekOfYear = dateRepository.GetCurrentWeek(DateTime.Now.Date);
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
                    ProgramId = x.UserPrograms.Select(y => y.MasterProgramId).FirstOrDefault(),
                    CoachId = x.UserPrograms.Select(y => y.CoachUserId).FirstOrDefault(),
                    ProgramUserId = x.UserId
                    
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

            var userProgram = userProgramRepository.GetActive.Where(x => x.ProgramUserId == userId).FirstOrDefault();
            var model = new UserProgramViewModel();
            if (userProgram == null)
            {
                var user = await UserManager.FindByIdAsync(userId);
                model.UserName = user.UserName;
                model.ProgramUserId = user.Id;
                ViewBag.IsNew = true;

            }
            else
            {
                model.UserName = userProgram.ProgramUser.UserName;
                model.Notes = "Some notes about this program";
                model.ProgramName = "TestProgramName";
                ViewBag.IsNew = false;
            };


            return View("UserProgramDetails", model);
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

    }
}