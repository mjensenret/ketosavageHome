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

            var clientProgress = _context.DailyProgress.Where(x => x.UserProgram.IsActive && !x.UserProgram.IsDeleted);

            if (!lifetime)
            {
                clientProgress = clientProgress.Where(x => x.Dates.ISOWeekOfYear == weekOfYear && x.Dates.Year == date.Year);
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
                    ActualCarbohydrate = (x.ActualCarbohydrate > x.PlannedCarbohydrate) ? x.ActualCarbohydrate : x.PlannedCarbohydrate
                })
                .ToList()
                .Select(y => new
                {
                    y.Name,
                    FatVariance = Math.Abs(Convert.ToDouble(y.ActualFat - y.PlannedFat)) * 9,
                    ProteinVariance = Math.Abs(Convert.ToDouble(y.ActualProtein - y.PlannedProtein)) * 4,
                    CarbVariance = Math.Abs(Convert.ToDouble(y.ActualCarbohydrate - y.PlannedCarbohydrate)) * 4
                })
                .GroupBy(s => new { s.Name })
                .Select(su => new
                {
                    su.Key.Name,
                    TotalVariance = su.Average(x => x.FatVariance + x.ProteinVariance + x.CarbVariance),
                    Fat = su.Sum(x => x.FatVariance),
                    Protein = su.Sum(x => x.ProteinVariance),
                    Carbs = su.Sum(x => x.CarbVariance)
                });
                //.GroupBy(s => new { s.Name })
                //.Select(g => new
                //{
                //    ClientName = g.Key.Name,
                //    FatVariance = (g.Average(x => Math.Abs(Convert.ToDouble(x.ActualFat - x.PlannedFat)))) * 9,
                //    ProtVariance = (g.Average(x => Math.Abs(Convert.ToDouble(x.ActualProtein - x.PlannedProtein)))) * 4,
                //    CarbVariance = (g.Average(x => Math.Abs(Convert.ToDouble(x.ActualCarbohydrate - x.PlannedCarbohydrate)))) * 4
                //})
                //.GroupBy(t => new { t.ClientName })
                //.Select(su => new
                //{
                //    su.Key.ClientName,
                //    TotalVariance = su.Sum(x => x.FatVariance + x.ProtVariance + x.CarbVariance)
                //});
            List<PerformanceChart> chart = new List<PerformanceChart>();
            foreach(var u in data.Take(5))
            {
                Debug.WriteLineIf(u.Name == "Michael Jensen", ($"--chart data u value: {u}"));

                var score = returnScore(Math.Abs(Convert.ToInt32(u.TotalVariance)));

                chart.Add(new PerformanceChart()
                {
                    ClientName = u.Name,
                    Score = score
                });
            }

            
            return chart;
        }

        private int returnScore(int totalVariance)
        {
            Debug.WriteLine($"returnScore-----------");
            Debug.WriteLine($"TotalVariance: {totalVariance}");
            return (totalVariance == 0 ) ? 0 :
                (totalVariance < 20) ? 10 :
                (totalVariance < 40) ? 9 :
                (totalVariance < 60) ? 8 :
                (totalVariance < 80) ? 7 :
                (totalVariance < 100) ? 6 :
                (totalVariance < 120) ? 5 :
                (totalVariance < 140) ? 4 :
                (totalVariance < 180) ? 3 :
                (totalVariance < 200) ? 2 :
                (totalVariance < 220) ? 1 :
                0;
        }
    }


}
