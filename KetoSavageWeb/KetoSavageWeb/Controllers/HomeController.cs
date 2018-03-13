using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KetoSavageWeb.Models;

namespace KetoSavageWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // DXCOMMENT: Pass a data model for GridView

            //return View(NorthwindDataProvider.GetCustomers());
            return View();
        }
        
        //public ActionResult GridViewPartialView() 
        //{
        //    // DXCOMMENT: Pass a data model for GridView in the PartialView method's second parameter
        //    return PartialView("GridViewPartialView", NorthwindDataProvider.GetCustomers());
        //}
    
    }

}

public enum HeaderViewRenderMode { Full, Title }