using KetoSavageWeb.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Models.Abstract
{
    public abstract class UserManaged : EntityBase<int>, IUserManaged, IHasIsNew
    {
        public UserManaged()
        {
            this.IsActive = true;
            this.IsDeleted = false;
            this.Created = DateTime.UtcNow;
            this.LastModified = this.Created;
        }

        public UserManaged(UserManaged copyFrom)
        {
            this.Id = copyFrom.Id;
            this.IsActive = copyFrom.IsActive;
            this.IsDeleted = copyFrom.IsDeleted;
            this.Created = copyFrom.Created;
            this.LastModified = copyFrom.LastModified;
            this.CreatedBy = copyFrom.CreatedBy;
            this.LastModifiedBy = copyFrom.LastModifiedBy;
        }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; }

        [Editable(false)]
        public DateTime Created { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime LastModified { get; set; }

        [Editable(false)]
        [Display(Name = "Created By"), MaxLength(50)]
        public string CreatedBy { get; set; }

        [Display(Name = "Last Modified By"), MaxLength(50)]
        public string LastModifiedBy { get; set; }

        public void SetModified(string ModifiedBy)
        {
            this.LastModified = DateTime.UtcNow;
            this.LastModifiedBy = ModifiedBy;
        }
    }
}