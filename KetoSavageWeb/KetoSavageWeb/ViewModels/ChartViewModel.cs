using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.ViewModels
{
    public class ChartViewModel
    {
    }

    public class WeightViewModel
    {
        public DateTime date { get; set; }
        public string type { get; set; }
        public double? weight { get; set; }
    }

    public class ProgressGaugeViewModel
    {
        public double actualWeight { get; set; }
        public double plannedWeight { get; set; }
    }

    public class WeeklyMacroPieChart
    {
        [Display(Name ="Macro")]
        public string macro { get; set; }
        [Display(Name ="Value")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double value { get; set; }

        [Display(Name ="Type")]
        public string type { get; set; }
        public double plannedFat { get; set; }
        public double plannedProt { get; set; }
        public double plannedCarbs { get; set; }
        public double actualFat { get; set; }
        public double actualProt { get; set; }
        public double actualCarbs { get; set; }
        public bool active { get; set; }
    }

    public class MacroPieChart
    {
        [Display(Name = "Macro")]
        public string macro { get; set; }
        [Display(Name = "Value")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double value { get; set; }

        [Display(Name = "Type")]
        public string type { get; set; }
        public double plannedFat { get; set; }
        public double plannedProt { get; set; }
        public double plannedCarbs { get; set; }
        public double actualFat { get; set; }
        public double actualProt { get; set; }
        public double actualCarbs { get; set; }
        public bool active { get; set; }
    }

    public class CurrentWeekMacroGauge
    {
        [Display(Name = "Macro")]
        public string macro { get; set; }
        [Display(Name = "Value")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double value { get; set; }

        [Display(Name = "Type")]
        public string type { get; set; }
        public double plannedFat { get; set; }
        public double plannedProt { get; set; }
        public double plannedCarbs { get; set; }
        public double actualFat { get; set; }
        public double actualProt { get; set; }
        public double actualCarbs { get; set; }
        public bool active { get; set; }
    }

    public class PvAMacroPieChart
    {
        [Display(Name = "Macro")]
        public string macro { get; set; }
        public int Planned { get; set; }
        public int Actual { get; set; }

    }

    public class ClientRenewalGrid
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public DateTime RenewalDate { get; set; }
    }

    public class ClientPerformanceData
    {
        public string ClientName { get; set; }
        public DateTime Date { get; set; }
        public double PlannedFat { get; set; }
        public double ActualFat { get; set; }
        public double PlannedProtein { get; set; }
        public double ActualProtein { get; set; }
        public double PlannedCarbs { get; set; }
        public double ActualCarbs { get; set; }
        public double FatVariance
        {
            get
            {
                return Math.Abs(PlannedFat - ActualFat);
            }
        }
        public double ProteinVariance
        {
            get
            {
                return Math.Abs(PlannedProtein - ActualProtein);
            }
        }
        public double CarbVariance
        {
            get
            {
                if(ActualCarbs > PlannedCarbs)
                {
                    return Math.Abs(PlannedCarbs - ActualCarbs) * 5;
                }
                else
                {
                    return 0;
                }
            }
        }
        public double TotalDailyVariance
        {
            get
            {
                return (FatVariance + ProteinVariance + CarbVariance);
            }
        }
    }

    public class ClientPerformanceScore
    {
        public string ClientName { get; set; }
        public int Score { get; set; }
    }
}