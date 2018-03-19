﻿using KetoSavageWeb.Models.Abstract;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models
{
    public class ProgramTemplate : UserManaged
    {
        public string Name { get; set; }
        public string programDescription { get; set; }
        public int GoalId { get; set; }
        public virtual ProgramGoals Goal { get; set; }
        public ICollection<UserPrograms> UserPrograms { get; set; }
    }

    public class ProgramGoals
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<ProgramTemplate> Programs { get; set; }
    }

}