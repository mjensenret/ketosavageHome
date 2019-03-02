using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using KetoSavageWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.XtraEditors.Filtering.Templates;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;
using MeasurementEntriesViewModel = KetoSavageWeb.ViewModels.MeasurementEntriesViewModel;
using DevExtreme.AspNet.Mvc;

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

        //public PartialViewResult MacroHeader()
        //{
        //    return PartialView("_dxMacroFormHeader");
        //}

        public PartialViewResult DxMacroPopup()
        {
            var model = userProgramRepository.GetDailyProgressByDate(CurrentUser.Id, DateTime.Now.Date);
            var userId = CurrentUser.Id;

            EnterMacroViewModel viewModel = new EnterMacroViewModel();

            viewModel.macroUserId = userId;
            viewModel.macroDate = model.Dates.Date;
            viewModel.actualFat = (model.ActualFat != null) ? Convert.ToDouble(model.ActualFat) : 0;
            viewModel.actualProtein = (model.ActualProtein != null) ? Convert.ToDouble(model.ActualProtein) : 0;
            viewModel.actualCarb = (model.ActualCarbohydrate != null) ? Convert.ToDouble(model.ActualCarbohydrate) : 0;
            viewModel.hungerLevelId = (model.HungerLevelId != null) ? Convert.ToInt32(model.HungerLevelId) : 0;
            viewModel.Notes = model.Notes;
            viewModel.masterProgramId = model.UserProgram.MasterProgramId;

            return PartialView("_dxMacroForm", viewModel);
        }

        //public PartialViewResult EnterMacroForm()
        //{
        //    var model = userProgramRepository.GetDailyProgressByDate(CurrentUser.Id, DateTime.Now.Date);
        //    var userId = CurrentUser.Id;

        //    EnterMacroViewModel viewModel = new EnterMacroViewModel();
            
        //    viewModel.macroUserId = userId;
        //    viewModel.macroDate = model.Dates.Date;
        //    viewModel.actualFat = (model.ActualFat != null) ? Convert.ToDouble(model.ActualFat) : 0 ;
        //    viewModel.actualProtein = (model.ActualProtein != null) ? Convert.ToDouble(model.ActualProtein) : 0;
        //    viewModel.actualCarb = (model.ActualCarbohydrate != null) ? Convert.ToDouble(model.ActualCarbohydrate) : 0;
        //    viewModel.hungerLevelId = (model.HungerLevelId != null) ? Convert.ToInt32(model.HungerLevelId) : 0;
        //    viewModel.Notes = model.Notes;
        //    viewModel.masterProgramId = model.UserProgram.MasterProgramId;



        //    return PartialView("_enterDailyMacros", viewModel);
        //}

        [HttpPost]
        public JsonResult EnterMacroForm(DateTime date)
        {
            var model = userProgramRepository.GetDailyProgressByDate(CurrentUser.Id, date);
            var userId = CurrentUser.Id;

            EnterMacroViewModel viewModel = new EnterMacroViewModel();

            viewModel.macroUserId = userId;
            viewModel.macroDate = model.Dates.Date;
            viewModel.actualFat = (model.ActualFat != null) ? Convert.ToDouble(model.ActualFat) : 0;
            viewModel.actualProtein = (model.ActualProtein != null) ? Convert.ToDouble(model.ActualProtein) : 0;
            viewModel.actualCarb = (model.ActualCarbohydrate != null) ? Convert.ToDouble(model.ActualCarbohydrate) : 0;
            viewModel.hungerLevelId = (model.HungerLevelId != null) ? Convert.ToInt32(model.HungerLevelId) : 0;
            viewModel.Notes = model.Notes;

            return Json(viewModel, JsonRequestBehavior.AllowGet);
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
                dailyProgress.HungerLevelId = model.hungerLevelId;
                dailyProgress.Notes = model.Notes;
                dailyProgress.LastModified = DateTime.Now;
                dailyProgress.LastModifiedBy = CurrentUser.UserName;

            }
            userProgramRepository.Update(userProgram);

            return RedirectToAction("Index");
        }

        public PartialViewResult MeasurementHeader()
        {
            return PartialView("_dxMeasurementHeader");
        }

        public PartialViewResult DxMeasurementForm()
        {
            var date = DateTime.Now.Date;
            var userProgram = userProgramRepository.GetActive.Where(p => p.ProgramUserId == CurrentUser.Id).Include(d => d.Measurements).FirstOrDefault();
            var userMeasurements = userProgram.Measurements.Where(x => x.Dates.Date == date);
            var userMeasurementDetails = userMeasurements.SelectMany(x => x.MeasurementDetails);
            var model = new MeasurementViewModel();

            model.MeasurementDate = date;
            model.UserProgramId = userProgram.Id;
            if (userMeasurements != null)
            {
                model.Id = userMeasurements.Select(m => m.Id).FirstOrDefault();
                model.MeasurementNotes = userMeasurements.Select(m => m.MeasurementNotes).FirstOrDefault();
                if (userMeasurementDetails.Any())
                {
                    var detailsVm = userMeasurementDetails
                        .Select(x => new
                        {
                            x.Id,
                            x.measurementHeaderId,
                            x.measurementType,
                            x.measurementValue
                        })
                        .ToList()
                        .Select(m => new MeasurementEntriesViewModel()
                        {
                            Id = m.Id,
                            MeasurementId = m.measurementHeaderId,
                            MeasurementType = m.measurementType,
                            MeasurementValue = m.measurementValue,
                            //MeasurementDropDown = new List<MeasurementType>()
                        });
                    model.MeasurementDetails = detailsVm;
                }

            }

            return PartialView("_dxMeasurementForm", model);
        }

        [HttpPost]
        public JsonResult DxUpdateMeasurementDateChange(DateTime newDate)
        {
            var date = newDate;
            var userProgram = userProgramRepository.GetActive.Where(p => p.ProgramUserId == CurrentUser.Id).Include(d => d.Measurements).FirstOrDefault();
            var userMeasurements = userProgram.Measurements.Where(x => x.Dates.Date == date);
            var userMeasurementDetails = userMeasurements.SelectMany(x => x.MeasurementDetails);
            var model = new MeasurementViewModel();

            model.MeasurementDate = date;
            model.UserProgramId = userProgram.Id;
            if (userMeasurements != null)
            {
                model.Id = userMeasurements.Select(m => m.Id).FirstOrDefault();
                model.MeasurementNotes = userMeasurements.Select(m => m.MeasurementNotes).FirstOrDefault();
                if (userMeasurementDetails.Any())
                {
                    var detailsVm = userMeasurementDetails
                        .Select(x => new
                        {
                            x.Id,
                            x.measurementHeaderId,
                            x.measurementType,
                            x.measurementValue
                        })
                        .ToList()
                        .Select(m => new MeasurementEntriesViewModel()
                        {
                            Id = m.Id,
                            MeasurementId = m.measurementHeaderId,
                            MeasurementType = m.measurementType,
                            MeasurementValue = m.measurementValue,
                            //MeasurementDropDown = new List<MeasurementType>()
                        });
                    model.MeasurementDetails = detailsVm;
                }

            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        // Fetching items from the "Orders" collection
        public ActionResult GetOrderDetails(DataSourceLoadOptions loadOptions, DateTime date)
        {
            IEnumerable<MeasurementEntriesViewModel> model = null;

            var header = _context.MeasurementHeader.Where(h => h.Dates.Date == date).FirstOrDefault();

            if (header != null)
            {
                var details = _context.MeasurementDetail.Where(d => d.measurementHeaderId == header.Id);


                if (details.Any())
                {
                    model = details
                        .Select(x => new
                        {
                            x.Id,
                            x.measurementHeaderId,
                            x.measurementType,
                            x.measurementValue
                        })
                        .ToList()
                        .Select(m => new MeasurementEntriesViewModel()
                        {
                            Id = m.Id,
                            MeasurementId = m.measurementHeaderId,
                            MeasurementType = m.measurementType,
                            MeasurementValue = m.measurementValue,
                            //MeasurementDropDown = new List<MeasurementType>()
                        });
                }

            }
            else
            {
                model = new List<MeasurementEntriesViewModel>();
            }



            var result = DataSourceLoader.Load(model, loadOptions);
            var resultJson = JsonConvert.SerializeObject(result);
            return Content(resultJson, "application/json");
        }

        [HttpPost]
        public ActionResult DxUpdateMeasurements(MeasurementViewModel model)
        {
            

            var measurementList = _context.MeasurementHeader.Where(x => x.UserProgramId == model.UserProgramId && x.Dates.Date == model.MeasurementDate).Include(y => y.MeasurementDetails);
            var dateId = _context.DateModels.Where(x => x.Date == model.MeasurementDate).Select(y => y.DateKey).Single();
            var dailyProgress = _context.DailyProgress.Single(x => x.UserProgramId == model.UserProgramId && x.DateId == dateId);
            
            if (measurementList.Any())
            {
                var updHeader = new MeasurementHeader();
                updHeader.Id = model.Id;
                updHeader.DateId = dateId;
                updHeader.UserProgramId = model.UserProgramId;
                updHeader.MeasurementNotes = model.MeasurementNotes;

                _context.MeasurementHeader.AddOrUpdate(updHeader);

                var updDetails = new MeasurementDetails();

                foreach (var i in model.MeasurementDetails)
                {
                    updDetails.Id = i.Id;
                    updDetails.measurementHeaderId = measurementList.Select(x => x.Id).FirstOrDefault();
                    updDetails.measurementType = i.MeasurementType;
                    updDetails.measurementValue = i.MeasurementValue;
                    if (i.MeasurementType == "Weight")
                    {
                        dailyProgress.ActualWeight = i.MeasurementValue;
                    }
                    _context.MeasurementDetail.AddOrUpdate(updDetails);
                }
                _context.SaveChanges();

            }
            else
            {
                var insModel = new MeasurementHeader();
                insModel.DateId = dateId;
                insModel.MeasurementNotes = model.MeasurementNotes;
                insModel.UserProgramId = model.UserProgramId;
                _context.MeasurementHeader.Add(insModel);
                
                foreach (var m in model.MeasurementDetails)
                {
                    var detModel = new MeasurementDetails();
                    detModel.measurementHeaderId = insModel.Id;
                    detModel.measurementType = m.MeasurementType;
                    detModel.measurementValue = m.MeasurementValue;
                    if (m.MeasurementType == "Weight")
                    {
                        dailyProgress.ActualWeight = m.MeasurementValue;
                    }
                    _context.MeasurementDetail.Add(detModel);
                }



                _context.SaveChanges();

            }

            return RedirectToAction("Index");
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
                return RedirectToAction("DxMeasurementForm");
            }

        }
    }
}