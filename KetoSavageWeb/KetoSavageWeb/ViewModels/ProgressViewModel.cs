﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.ViewModels
{
    public class ProgressViewModel
    {
    }
    public class EnterMacroViewModel
    {
        public int macroUserId { get; set; }
        public int macroProgressId { get; set; }
        public DateTime macroDate { get; set; }
        public double actualFat { get; set; }
        public double actualProtein { get; set; }
        public double actualCarb { get; set; }
    }

    public class EnterMeasurementViewModel
    {
        public int measurementUserId { get; set; }
        public int measurementProgressId { get; set; }
        public DateTime measurementDate { get; set; }
        public double actualWeight { get; set; }
    }
}