using KetoSavageWeb.Models.Abstract;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class ProgramTemplate : UserManaged
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        [Display(Name="Weight Factor")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? WeightWeek { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int GoalId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual ProgramGoals Goal { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<UserPrograms> UserPrograms { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<HungerLevel> HungerLevel { get; set; }
    }

    public class ProgramGoals
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [Display(Name = "Category")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<ProgramTemplate> Programs { get; set; }
    }

    public class HungerLevel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Value { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int programId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual ProgramTemplate Program { get; set; }
    }

}