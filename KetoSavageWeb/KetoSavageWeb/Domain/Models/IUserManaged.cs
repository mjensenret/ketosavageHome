using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Domain.Models
{
    public interface IUserManaged
    {
        bool IsActive { get; set; }
        bool IsDeleted { get; set; }

        DateTime Created { get; set; }
        DateTime LastModified { get; set; }
        string CreatedBy { get; set; }
        string LastModifiedBy { get; set; }
    }

    public interface IHasIsNew
    {
        bool IsNew { get; }
    }
}