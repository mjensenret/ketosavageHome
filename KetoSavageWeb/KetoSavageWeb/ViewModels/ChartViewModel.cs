using System;
using System.Collections.Generic;
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
}