using KetoSavageWeb.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace KetoSavageWeb.Domain.Repositories
{
    public class GenericRepository<TEntity> : GenericRepository<TEntity, int>
        where TEntity : class
    {
        public GenericRepository(IEntityContext<TEntity> entityContext) : base(entityContext) { }
    }

    /// <summary>
    /// Implementation of the repository pattern that uses an injected data context.
    /// Designed to support custom repositories that can be tested with mock data.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class GenericRepository<TEntity, TKey> : IDisposable
        where TEntity : class
    {
        protected IEntityContext<TEntity, TKey> entityContext;

        public GenericRepository(IEntityContext<TEntity, TKey> entityContext)
        {
            this.entityContext = entityContext;
        }

        public virtual void Dispose()
        {
            entityContext.Dispose();
        }

        /// <summary>
        /// Get a queryable entity collection
        /// </summary>
        public virtual IQueryable<TEntity> Get
        {
            get { return entityContext.Get; }
        }

        public bool SupportsTransactions
        {
            get { return entityContext.SupportsTransactions; }
        }

        public virtual Task<TEntity> FindAsync(TKey Id)
        {
            return entityContext.FindAsync(Id);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await entityContext.UpdateAsync(entity);
            return entity;
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            await entityContext.CreateAsync(entity);
            return entity;
        }

        public virtual Task DeleteAsync(TEntity entity)
        {
            return entityContext.DeleteAsync(entity);
        }

        public TEntity Find(TKey Id)
        {
            return AsyncHelper.RunSync(() => FindAsync(Id));
        }

        /// <summary>
        ///  calls either create or update as needed based on if the id field is 0 
        /// </summary>
        /// <param name="idKey">id of the record in question</param>
        /// <param name="entity">object to save to the database</param>
        /// <param name="isActive">object to save to the database</param>
        /// <returns></returns>
        public TEntity CreateOrUpdate(int idKey, TEntity entity, bool isActive = true)
        {
            //if key = 0 its an insert, and we don't insert non active records
            if (idKey == 0 && !isActive)
                return null;
            return idKey == 0 ? Create(entity) : Update(entity);
        }

        public TEntity Update(TEntity entity)
        {
            AsyncHelper.RunSync(() => UpdateAsync(entity));
            return entity;
        }

        public TEntity Create(TEntity entity)
        {
            AsyncHelper.RunSync(() => CreateAsync(entity));
            return entity;
        }

        public void Delete(TEntity entity)
        {
            AsyncHelper.RunSync(() => DeleteAsync(entity));
        }

        public void BeginTransaction()
        {
            entityContext.BeginTransaction();
        }

        public void CommitTransaction()
        {
            entityContext.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            entityContext.RollbackTransaction();
        }
    }

    internal static class AsyncHelper
    {
        private static readonly TaskFactory _myTaskFactory = new TaskFactory(CancellationToken.None,
            TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            var cultureUi = CultureInfo.CurrentUICulture;
            var culture = CultureInfo.CurrentCulture;
            return _myTaskFactory.StartNew(() =>
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = cultureUi;
                return func();
            }).Unwrap().GetAwaiter().GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            var cultureUi = CultureInfo.CurrentUICulture;
            var culture = CultureInfo.CurrentCulture;
            _myTaskFactory.StartNew(() =>
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = cultureUi;
                return func();
            }).Unwrap().GetAwaiter().GetResult();
        }
    }
}