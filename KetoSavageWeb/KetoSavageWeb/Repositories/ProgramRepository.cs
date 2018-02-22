using KetoSavageWeb.Models;
using KetoSavageWeb.Models.Contexts;
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
    }
}