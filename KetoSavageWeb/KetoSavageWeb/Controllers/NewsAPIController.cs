using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using KetoSavageWeb.Controllers.Abstract;
using KetoSavageWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;

namespace KetoSavageWeb.Controllers
{
    public class NewsAPIController : KSBaseAPIController
    {
        private KSDataContext mcontext;

        public NewsAPIController(KSDataContext context)
        {
            mcontext = context;
        }

        [HttpGet]
        public HttpResponseMessage GetNews(DataSourceLoadOptions loadOptions)
        {
            var newsList = mcontext.NewsModel.Where(x => x.IsActive);

            return Request.CreateResponse(DataSourceLoader.Load(newsList, loadOptions));
        }

        [HttpGet]
        public HttpResponseMessage GetNewsTypes(DataSourceLoadOptions loadOptions)
        {
            var newsTypeList = new List<object>();

            newsTypeList.Add(new { Text = "News", Value = "News" });
            newsTypeList.Add(new { Text = "Quote", Value = "Quote" });

            return Request.CreateResponse(DataSourceLoader.Load(newsTypeList, loadOptions));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> AddNews(FormDataCollection form)
        {
            var values = form.Get("values");

            var newsItem = new NewsModel();
            JsonConvert.PopulateObject(values, newsItem);

            mcontext.NewsModel.Add(newsItem);
            var result = await mcontext.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.Created);
        }

        [HttpPut]
        public async Task<HttpResponseMessage> ModifyNews(FormDataCollection form)
        {
            var values = form.Get("values");
            var key = Convert.ToInt32(form.Get("key"));
            var updNews = await mcontext.NewsModel.FindAsync(key);
            JsonConvert.PopulateObject(values, updNews);

            var result = await mcontext.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteNews(FormDataCollection form)
        {
            var key = Convert.ToInt32(form.Get("key"));
            var delNews = await mcontext.NewsModel.FindAsync(key);
            if (delNews != null)
            {
                mcontext.NewsModel.Remove(delNews);
                var result = mcontext.SaveChangesAsync();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something went wrong deleting the news article");
            
        }
    }
}
