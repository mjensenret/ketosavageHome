using KetoSavageWeb.Domain.Infrastructure;
using KetoSavageWeb.Domain.Repositories;
using KetoSavageWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Repositories
{
    public class RoleRepository : GenericRepository<Role>
    {
        public RoleRepository(IEntityContext<Role> entityContext) : base(entityContext) { }
    }
}