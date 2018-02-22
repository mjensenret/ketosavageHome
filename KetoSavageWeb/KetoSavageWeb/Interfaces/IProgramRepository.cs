using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KetoSavageWeb.Models;

namespace KetoSavageWeb.Interfaces
{
    public interface IProgramRepository
    {
        IEnumerable<ProgramModels> SelectAll();
        ProgramModels SelectByUserId(int userId);
        void Insert(ProgramModels obj);
        void Update(ProgramModels obj);
        void Delete(string Id);
        void Save();

    }
}