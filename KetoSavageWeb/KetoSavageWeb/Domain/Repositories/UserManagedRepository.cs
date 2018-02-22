using KetoSavageWeb.Domain.Infrastructure;
using KetoSavageWeb.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KetoSavageWeb.Domain.Repositories
{
    public class UserManagedRepository<TEntity> : UserManagedRepository<TEntity, int>
        where TEntity : class, IUserManaged
    {
        public UserManagedRepository(IEntityContext<TEntity> entityContext) : base(entityContext) { }
    }

    public class UserManagedRepository<TEntity, TKey> : GenericRepository<TEntity, TKey>
        where TEntity : class, IUserManaged
    {
        public UserManagedRepository(IEntityContext<TEntity, TKey> entityContext) : base(entityContext) { }

        /// <summary>
        /// Get all data that is not deleted. Includes data where IsActive = false.
        /// </summary>
        public override IQueryable<TEntity> Get
        {
            get { return base.Get.Where(m => m.IsDeleted == false); }
        }

        /// <summary>
        /// Get active data that is not deleted.
        /// </summary>
        public virtual IQueryable<TEntity> GetActive
        {
            get { return this.Get.Where(m => m.IsActive == true); }
        }

        /// <summary>
        /// Get all data, including deleted records.
        /// </summary>
        public virtual IQueryable<TEntity> GetAll
        {
            get { return base.Get; }
        }

        // Override behavior

        public override Task DeleteAsync(TEntity entity)
        {
            entity.IsDeleted = true;
            return base.UpdateAsync(entity);
        }

        // Add methods to set CreatedBy/LastModifiedBy

        public virtual Task<TEntity> CreateAsync(TEntity entity, string CreatedBy)
        {
            entity.CreatedBy = CreatedBy;
            entity.LastModified = DateTime.UtcNow;
            entity.LastModifiedBy = CreatedBy;
            return this.CreateAsync(entity);
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity, string ModifiedBy)
        {
            entity.LastModified = DateTime.UtcNow;
            entity.LastModifiedBy = ModifiedBy;
            return this.UpdateAsync(entity);
        }

        public virtual Task DeleteAsync(TEntity entity, string RemovedBy)
        {
            entity.LastModified = DateTime.UtcNow;
            entity.LastModifiedBy = RemovedBy;
            return this.DeleteAsync(entity);
        }

        public virtual TEntity Create(TEntity entity, string CreatedBy)
        {
            entity.CreatedBy = CreatedBy;
            entity.LastModified = DateTime.UtcNow;
            entity.LastModifiedBy = CreatedBy;
            return this.Create(entity);
        }

        public virtual TEntity Update(TEntity entity, string ModifiedBy)
        {
            entity.LastModified = DateTime.UtcNow;
            entity.LastModifiedBy = ModifiedBy;
            return this.Update(entity);
        }

        public virtual void Remove(TEntity entity, string RemovedBy)
        {
            entity.LastModified = DateTime.UtcNow;
            entity.LastModifiedBy = RemovedBy;
            this.Delete(entity);
        }
    }
}