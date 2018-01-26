using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Util.Interfaces
{
    public interface IDataLayer<TModel> : IDisposable where TModel : class
    {
        /// <summary>
        /// Get a queryable entity collection
        /// </summary>
        /// <returns></returns>
        IQueryable<TModel> Get { get; }

        /// <summary>
        /// Find an object by key lookup
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        TModel Find(params object[] keys);

        /// <summary>
        /// Add a new object to the repository
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        TModel Add(TModel model);

        /// <summary>
        /// Save a modified entity
        /// </summary>
        /// <param name="t"></param>
        TModel Update(TModel model);

        /// <summary>
        /// Remove an object from the repository
        /// </summary>
        /// <param name="t"></param>
        void Remove(TModel model);

        /// <summary>
        /// Rollback an entity to origonal values
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        TEntity UndoChanges<TEntity>(TEntity model) where TEntity : class;

        /// <summary>
        /// Return true if the specified model being tracked by the data layer.
        /// Some implementations will always return false.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool IsAttached(TModel model);  // TODO: rename IsTracked?

        bool SupportsTransactions { get; }

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

    }
}