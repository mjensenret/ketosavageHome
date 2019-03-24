using KetoSavageWeb.Models;
using KetoSavageWeb.Repositories;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace KetoSavageWeb.Controllers
{
    public class ManageNewsController : Controller
    {
        private KSDataContext _context = new KSDataContext();

        //GET News grid
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult NewsGridPartial()
        {

            return PartialView("_dxNewsGrid");

        }

        public async Task<ActionResult> NewsGridPartialAddNew(NewsModel item)
        {
            if (ModelState.IsValid)
            {
                var newNews = new NewsModel
                {
                    Headline = item.Headline,
                    Author = item.Author,
                    IsActive = true,
                    Expires = item.Expires,
                    Type = item.Type

                };

                try
                {
                    _context.NewsModel.Add(newNews);
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = e.Message.First();
                    ViewData["EditError"] = e.Message.First();
                }
            }
            else
            {
                ViewData["EditError"] = "Please correct all errors";
            }

            var newsTypes = getTypeList();

            ViewBag.NewsType = getTypeList();

            var model = _context.NewsModel.Where(x => x.IsActive);

            return PartialView("_newsGrid", model);
        }

        
        public async Task<ActionResult> NewsGridPartialEdit(NewsModel item)
        {
            if (ModelState.IsValid)
            {
                var editNews = _context.NewsModel.Where(x => x.Id == item.Id).FirstOrDefault();

                if(editNews == null)
                {
                    return HttpNotFound();
                }

                try
                {
                    editNews.Headline = item.Headline;
                    editNews.Type = item.Type;
                    editNews.IsActive = item.IsActive;
                    editNews.Author = item.Author;
                    editNews.Expires = item.Expires;

                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = e.Message.First();
                    ViewData["EditError"] = e.Message.First();
                }
            }
            else
            {
                ViewData["EditError"] = "Please correct all errors";
            }

            ViewBag.NewsType = getTypeList();

            var model = _context.NewsModel.Where(x => x.IsActive);

            return PartialView("_newsGrid", model);
        }

        public PartialViewResult newsCarousel()
        {
            var activeNews = _context.NewsModel.Where(x => x.IsActive);

            var test = activeNews.Count();

            return PartialView("_newsCarousel", activeNews);
        }

        private SelectList getTypeList()
        {
            var types = new SelectList(
                        new List<SelectListItem>
                        {
                            new SelectListItem { Selected = true, Text = "News", Value = "News"},
                            new SelectListItem { Selected = false, Text = "Quote", Value = "Quote"},
                        }, "Value", "Text", 1);

            return types;
        }

    }
}