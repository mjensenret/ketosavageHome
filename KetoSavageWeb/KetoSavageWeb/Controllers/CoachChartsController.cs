using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb.Controllers
{
    public class CoachChartsController : Controller
    {
        // GET: CoachCharts
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult ClientRenewalGrid()
        {
            return PartialView("_clientRenewalGrid");
        }

        public PartialViewResult TopPerformers()
        {
            return PartialView("_topPerformers");
        }

        public PartialViewResult BottomPerformers()
        {
            return PartialView("_bottomPerformers");
        }

        public PartialViewResult TopPerformersLifetime()
        {
            return PartialView("_topPerformersLifetime");
        }

        public PartialViewResult BottomPerformersLifetime()
        {
            return PartialView("_bottomPerformersLifetime");
        }

    }
}