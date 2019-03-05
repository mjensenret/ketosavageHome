using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using KetoSavageWeb.Models;
using KetoSavageWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KetoSavageWeb.Controllers
{
    public class ChartsAPIController : ApiController
    {
        KSDataContext _context = new KSDataContext();

        public ChartsAPIController()
        {

        }

        [HttpGet]
        public HttpResponseMessage GetClientRenewals(DataSourceLoadOptions loadOptions)
        {
            var today = DateTime.Now.Date;
            var clients = _context.UserPrograms.Where(x => x.IsActive && !x.IsDeleted && x.RenewalDate <= today);

            var model = clients
                .Select(x => new
                {
                    ClientName = x.ProgramUser.FirstName + " " + x.ProgramUser.LastName,
                    x.RenewalDate
                })
                .ToList()
                .Select(y => new ClientRenewalGrid()
                {
                    ClientName = y.ClientName,
                    RenewalDate = Convert.ToDateTime(y.RenewalDate)
                });

            return Request.CreateResponse(DataSourceLoader.Load(model, loadOptions));
        }

        [HttpGet]
        public HttpResponseMessage GetComplianceScore(DataSourceLoadOptions loadOptions, string type, bool lifetime)
        {

            var date = DateTime.Now.Date;
            var weekOfYear = _context.DateModels.Where(x => x.Date == date).Select(y => y.ISOWeekOfYear).First();

            var clientProgress = _context.DailyProgress.Where(x => x.IsActive && !x.IsDeleted && x.Dates.Date < date && x.UserProgram.EndDate > date);

            var variance = clientProgress
                .OrderBy(x => x.UserProgram.ProgramUser.FirstName)
                .ThenBy(x => x.Dates.Date)
                .Select(x => new
                {
                    User = x.UserProgram.ProgramUser.FirstName + " " + x.UserProgram.ProgramUser.LastName,
                    x.Dates.Date,
                    x.PlannedFat,
                    x.ActualFat,
                    x.PlannedProtein,
                    x.ActualProtein,
                    x.PlannedCarbohydrate,
                    x.ActualCarbohydrate
                })
                .ToList()
                .Select(y => new ClientPerformanceData()
                {
                    ClientName = y.User,
                    Date = y.Date,
                    PlannedFat = Convert.ToDouble(y.PlannedFat),
                    ActualFat = Convert.ToDouble(y.ActualFat),
                    PlannedProtein = Convert.ToDouble(y.PlannedProtein),
                    ActualProtein = Convert.ToDouble(y.ActualProtein),
                    PlannedCarbs = Convert.ToDouble(y.PlannedCarbohydrate),
                    ActualCarbs = Convert.ToDouble(y.ActualCarbohydrate)
                });

            if (!lifetime)
                variance = variance.Where(x => x.Date.Date >= date.AddDays(-7) && x.Date.Date < date);

            var scoreModel = returnVarianceScore(variance);

            if(type == "top")
            {
                scoreModel = scoreModel.Where(x => x.Score > 5).OrderByDescending(x => x.Score).ToList();
            }
            else
            {
                scoreModel = scoreModel.Where(x => x.Score <= 5).OrderBy(x => x.Score).ToList();
            }

            return Request.CreateResponse(DataSourceLoader.Load(scoreModel.Take(5), loadOptions));


        }

        private List<ClientPerformanceScore> returnVarianceScore(IEnumerable<ClientPerformanceData> pd)
        {
            var data = pd
                .Select(x => new
                {
                    Name = x.ClientName,
                    x.TotalDailyVariance
                })
                .ToList()
                .GroupBy(s => new { s.Name })
                .Select(su => new
                {
                    su.Key.Name,
                    AvgVariance = su.Average(x => x.TotalDailyVariance)
                });

            List<ClientPerformanceScore> chart = new List<ClientPerformanceScore>();
            foreach(var u in data)
            {

                var score = returnScore(Convert.ToInt32(u.AvgVariance));

                chart.Add(new ClientPerformanceScore()
                {
                    ClientName = u.Name,
                    Score = score
                });
            }

            
            return chart;
        }

        private int returnScore(int totalVariance)
        {
            return (totalVariance == 0 ) ? 0 :
                (totalVariance < 10) ? 10 :
                (totalVariance < 20) ? 9 :
                (totalVariance < 30) ? 8 :
                (totalVariance < 40) ? 7 :
                (totalVariance < 50) ? 6 :
                (totalVariance < 60) ? 5 :
                (totalVariance < 70) ? 4 :
                (totalVariance < 80) ? 3 :
                (totalVariance < 90) ? 2 :
                (totalVariance < 100) ? 1 :
                0;
        }


    }


}
