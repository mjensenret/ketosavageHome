using KetoSavageWeb.Domain.Models;
using KetoSavageWeb.Infrastructure;
using KetoSavageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb.ViewModels
{
    public class ProgramViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string GoalName { get; set; }
        public int GoalId { get; set; }
    }
    public class ProgramListViewModel : UserManagedListModel<ProgramViewModel>
    {
        public int GoalId { get; set; }
        public string GoalName { get; set; }
        //public string SelectedGoalId { get; set; }
        public SelectList GoalList { get; set; }
        public override void SaveState(WriteCookieDelegate writeCookie)
        {
            base.SaveState(writeCookie);
        }
    }

    public class ProgramEditViewModel : IKeyedEntity<int>, IHasIsNew
    {
        public ProgramTemplate Program { get; set; }
        public SelectList GoalList { get; set; }
        public string Name
        {
            get { return this.Program.Name; }
            set { this.Program.Name = value; }
        }
        public int Id
        {
            get { return this.Program.Id; }
            set { this.Program.Id = value; }
        }

        public string Description
        {
            get { return this.Program.programDescription; }
            set { this.Program.programDescription = value; }
        }
        public int GoalId
        {
            get { return this.Program.GoalId; }
            set { this.Program.GoalId = value; }
        }

        public bool IsActive
        {
            get { return this.Program.IsActive; }
            set { this.Program.IsActive = value; }
        }
        public bool IsNew
        {
            get { return this.IsNew; }
        }

        public ProgramEditViewModel()
        {

        }

    }
}