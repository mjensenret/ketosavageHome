using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using KetoSavageWeb.Models;
using KetoSavageWeb.ViewModels;
using System;
using System.Collections.Generic;
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

            var clientProgress = _context.DailyProgress.Where(x => x.UserProgram.IsActive && !x.UserProgram.IsDeleted);

            if (!lifetime)
            {
                clientProgress = clientProgress.Where(x => x.Dates.ISOWeekOfYear == weekOfYear);
            }


            var scoreModel = returnVarianceScore(clientProgress);

            if(type == "top")
            {
                scoreModel = scoreModel.Where(x => x.Score >= 7).ToList();
            }
            else
            {
                scoreModel = scoreModel.Where(x => x.Score <= 5).ToList();
            }

            scoreModel.Take(5);

            return Request.CreateResponse(DataSourceLoader.Load(scoreModel, loadOptions));


        }

        private List<PerformanceChart> returnVarianceScore(IQueryable<DailyProgress> dp)
        {
            var data = dp
                .Select(x => new
                {
                    Name = (x.UserProgram.ProgramUser.FirstName + " " + x.UserProgram.ProgramUser.LastName),
                    x.PlannedFat,
                    x.PlannedProtein,
                    x.PlannedCarbohydrate,
                    x.ActualFat,
                    x.ActualProtein,
                    x.ActualCarbohydrate
                })
                .GroupBy(s => new { s.Name })
                .Select(g => new
                {
                    ClientName = g.Key.Name,
                    FatVariance = (g.Average(x => x.ActualFat - x.PlannedFat)) * 9,
                    ProtVariance = (g.Average(x => x.ActualProtein - x.PlannedProtein)) * 4,
                    CarbVariance = (g.Average(x => x.ActualCarbohydrate - x.PlannedCarbohydrate)) * 4
                })
                .GroupBy(t => new { t.ClientName })
                .Select(su => new
                {
                    su.Key.ClientName,
                    TotalVariance = su.Sum(x => x.FatVariance + x.ProtVariance + x.CarbVariance)
                });
            List<PerformanceChart> chart = new List<PerformanceChart>();
            foreach(var u in data)
            {
                
                var score = returnScore(Math.Abs(Convert.ToInt32(u.TotalVariance)));
                chart.Add(new PerformanceChart()
                {
                    ClientName = u.ClientName,
                    Score = score
                });
            }
            
            return chart;
        }

        private int returnScore(int totalVariance)
        {
            return (totalVariance == 0 ) ? 0 :
                (totalVariance < 50) ? 10 :
                (totalVariance < 100) ? 9 :
                (totalVariance < 150) ? 8 :
                (totalVariance < 200) ? 7 :
                (totalVariance < 250) ? 6 :
                (totalVariance < 300) ? 5 :
                (totalVariance < 350) ? 4 :
                (totalVariance < 400) ? 3 :
                (totalVariance < 450) ? 2 :
                (totalVariance < 500) ? 1 :
                0;
        }
    }


}
