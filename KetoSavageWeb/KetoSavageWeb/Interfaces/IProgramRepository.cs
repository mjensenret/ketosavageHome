using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KetoSavageWeb.Models;

namespace KetoSavageWeb.Interfaces
{
    public interface IProgramRepository
    {
        IEnumerable<ProgramTemplate> SelectAll();
        ProgramTemplate SelectByUserId(int userId);
        void Insert(ProgramTemplate obj);
        void Update(ProgramTemplate obj);
        void Delete(string Id);
        void Save();

    }
}