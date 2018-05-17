using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KetoSavageWeb.Models;

namespace KetoSavageWeb.Controllers
{
    public class HomeController : KSBaseController
    {
        public ActionResult Index()
        {
            if(Request.IsAuthenticated)
            {
                var coachOrAdmin = CurrentUser.Roles.Where(x => x.Role.Name == "Coach" || x.Role.Name == "Administrator").FirstOrDefault();

                if (coachOrAdmin != null)
                {
                    ViewBag.IsCoachOrAdmin = true;
                }
                else
                {
                    ViewBag.IsCoachOrAdmin = false;
                }
            }


            return View();
        }
        
        public PartialViewResult mainMenuButtons()
        {
            var coachOrAdmin = CurrentUser.Roles.Where(x => x.Role.Name == "Coach" || x.Role.Name == "Administrator").FirstOrDefault();

            if (coachOrAdmin != null)
            {
                ViewBag.IsCoachOrAdmin = true;
            }
            else
            {
                ViewBag.IsCoachOrAdmin = false;
            }

            return PartialView("_mainMenuButtons");
        }

        [HttpPost]
        public ActionResult RedirectToPage(string buttonName)
        {
            var button = buttonName;

            switch (button)
            {
                case "btnEditUser":
                    //url = string.Format("\"Index\", \"ManageUsers\")");
                    RedirectToAction("Index", "ManageUsers");
                    break;
                case "btnAdminUpdateMacros":
                    RedirectToAction("Index", "ManageUsers");
                    break;
            }
            return null;


            
        }


    }

}

public enum HeaderViewRenderMode { Full, Title }