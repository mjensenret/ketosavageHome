using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KetoSavageWeb.Domain.Infrastructure
{
    public interface IEntityContext<TEntity> : IEntityContext<TEntity, int> where TEntity : class { }

    public interface IEntityContext<TEntity, TKey> : IDisposable
        where TEntity : class
    {
        /// <summary>
        /// Get a queryable entity collection
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> Get { get; }

        /// <summary>
        /// Find an object by key lookup
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<TEntity> FindAsync(TKey Id);

        /// <summary>
        /// Add a new object to the repository
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task CreateAsync(TEntity entity);

        /// <summary>
        /// Save a modified entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Remove an object from the repository
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(TEntity entity);

        bool SupportsTransactions { get; }

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
    }
}