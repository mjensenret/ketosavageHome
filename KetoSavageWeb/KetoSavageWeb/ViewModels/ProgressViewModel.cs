using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb.ViewModels
{
    public enum HungerLevel
    {
        None,
        Slight,
        Moderate,
        Sevier
    }

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
        public string hungerLevel { get; set; }
        public string Notes { get; set; }
        public List<SelectListItem> hungerList()
        {
            List<SelectListItem> choices = new List<SelectListItem>();
            foreach (var c in Enum.GetValues(typeof(HungerLevel)))
            {
                choices.Add(new SelectListItem() { Text = c.ToString(), Value = c.ToString() });
            }
            return choices;
        }
    }

    public class EnterMeasurementViewModel
    {
        public int measurementUserId { get; set; }
        public int measurementProgressId { get; set; }
        public DateTime measurementDate { get; set; }
        public double actualWeight { get; set; }
    }
}