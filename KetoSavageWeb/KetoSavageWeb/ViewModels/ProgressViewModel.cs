using KetoSavageWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb.ViewModels
{
    public enum MeasurementType
    {
        Weight,
        Neck,
        Shoulders,
        Chest,
        [Display(Name ="Right Arm")]
        RightArm,
        [Display(Name ="Left Arm")]
        LeftArm,
        Waist,
        Hips,
        [Display(Name = "Right Thigh")]
        RightThigh,
        [Display(Name = "Left Thigh")]
        LeftThigh,
        [Display(Name = "Right Calve")]
        RightCalve,
        [Display(Name = "Left Calve")]
        LeftCalve

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
        public int hungerLevelId { get; set; }
        public string Notes { get; set; }
        public int masterProgramId { get; set; }
    }

    public class EnterMeasurementViewModel
    {
        public int measurementUserId { get; set; }
        public int measurementProgressId { get; set; }
        public DateTime measurementDate { get; set; }
        public double actualWeight { get; set; }
    }

    public class MeasurementViewModel
    {
        public int Id { get; set; }
        public int UserProgramId { get; set; }
        public DateTime MeasurementDate { get; set; }
        public string MeasurementNotes { get; set; }
        public IEnumerable<MeasurementEntriesViewModel> MeasurementDetails { get; set; }
        public MeasurementType MeasurementTypes { get; set; }
    }

    public class MeasurementEntriesViewModel
    {
        public int Id { get; set; }
        public int MeasurementId { get; set; }
        public string MeasurementType { get; set; }
        public double MeasurementValue { get; set; }
        public MeasurementViewModel MeasurementHeader { get; set; }
        public List<MeasurementType> MeasurementDropDown { get; set; }
    }
}