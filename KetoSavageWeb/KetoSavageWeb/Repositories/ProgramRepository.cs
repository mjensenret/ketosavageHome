﻿using KetoSavageWeb.Models;
using KetoSavageWeb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KetoSavageWeb.Domain.Repositories;
using KetoSavageWeb.Domain.Infrastructure;

namespace KetoSavageWeb.Repositories
{
    public class ProgramRepository : UserManagedRepository<ProgramModels>
    {
        public ProgramRepository(IEntityContext<ProgramModels> entityContext) : base(entityContext)
        {
        }

        public IQueryable<ProgramModels> FindActiveProgramByUser(string userName)
        {
            return GetActive.Where(x => x.ProgramUser.UserName == userName);
        }

        public IQueryable<ProgramModels> GetByType(ProgramType type)
        {
            if (type == ProgramType.Coached)
            { 
                return GetActive.Where(x => x is CoachedPrograms);
            }
            else
            {
                return GetActive.Where(x => x is SelfGuidedPrograms);
            }
        }

        public void CreateDefaulClientProgram(string clientName, string coachName)
        {
            //var manager = ApplicationUserManager.Create(new KSDataContext());
            var updContext = new KSDataContext();
            
            var client = updContext.Users.Where(x => x.UserName == clientName).First();
            var coach = updContext.Users.Where(y => y.UserName == coachName).First();

            CoachedPrograms program = new CoachedPrograms
            {
                CoachUser = coach,
                ProgramUser = client,
                startDate = DateTime.Now,
                renewalDate = DateTime.Now.AddDays(30)
            };
            updContext.Programs.Add(program);
            try
            {
                updContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}