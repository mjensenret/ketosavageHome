using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KetoSavageWeb.Models;

namespace KetoSavageWeb.Controllers
{
    public class ClientDashboardController : Controller
    {


        // GET: ClientDashboard
        public ActionResult Index()
        {
            return View();
        }

    }
}
