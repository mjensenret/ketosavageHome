using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using KetoSavageWeb.ViewModels;

namespace KetoSavageWeb.Controllers
{
    public class ProgressApiController : ApiController
    {
        private ProgramRepository program;
        private RoleRepository roleRepository;
        private UserProgramRepository userProgramRepository;
        private DateRepository dateRepository;

        private KSDataContext _context = new KSDataContext();

        public ProgressApiController(ProgramRepository pr, RoleRepository rr, UserProgramRepository up, DateRepository dr)
        {
            this.program = pr;
            this.roleRepository = rr;
            this.userProgramRepository = up;
            this.dateRepository = dr;
        }

        //[Route("api/Measurements/{action}", Name = "Measurements")]
        [HttpGet]
        public HttpResponseMessage GetMeasurementDetails(int measurementId, DataSourceLoadOptions loadOptions)
        {
            IQueryable<MeasurementEntriesViewModel> measurementDetailsViewModel = null;

            measurementDetailsViewModel =
                _context.MeasurementDetail.Where(x => x.measurementHeaderId == measurementId).Select(x => new MeasurementEntriesViewModel()
                {
                    MeasurementId = x.measurementHeaderId,
                    MeasurementType = x.measurementType,
                    MeasurementValue = x.measurementValue
                });
           
            return Request.CreateResponse(DataSourceLoader.Load(measurementDetailsViewModel, loadOptions));
        }
    }
}
