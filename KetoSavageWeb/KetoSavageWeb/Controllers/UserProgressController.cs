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
            return View();
        }
        public PartialViewResult EnterMacroForm()
        {
            EnterMacroViewModel model = new EnterMacroViewModel();
            var userId = CurrentUser.Id;
            model.macroUserId = userId;
            model.macroDate = DateTime.Now;

            return PartialView("_enterDailyMacros", model);
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
                dailyProgress.ActualCarbohydrate = model.actualProtein;
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
        public ActionResult Weight()
        {
            return View();
        }
    }
}