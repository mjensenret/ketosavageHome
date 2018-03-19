using KetoSavageWeb.Models;
using KetoSavageWeb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KetoSavageWeb.Domain.Repositories;
using KetoSavageWeb.Domain.Infrastructure;

namespace KetoSavageWeb.Repositories
{
    public class ProgramRepository : UserManagedRepository<ProgramTemplate>
    {
        private KSDataContext _context = new KSDataContext();
        public ProgramRepository(IEntityContext<ProgramTemplate> entityContext) : base(entityContext)
        {
        }

        public IEnumerable<ProgramGoals> getGoals()
        {
            var goals = _context.ProgramGoals;
            return goals;
        }

        public ProgramGoals getGoalById(int goalId)
        {
            var selectedGoal = _context.ProgramGoals.Where(g => g.Id == goalId).FirstOrDefault();
            return selectedGoal;

        }


    }
}