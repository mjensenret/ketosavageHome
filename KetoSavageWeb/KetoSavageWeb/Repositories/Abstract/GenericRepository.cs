using KetoSavageWeb.Util.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Repositories.Abstract
{
    public class GenericRepository<TModel> : IDisposable where TModel : class
    {
        protected IDataLayer<TModel> dataLayer;

        public GenericRepository(IDataLayer<TModel> dataLayer)
        {
            this.dataLayer = dataLayer;
        }

        public virtual void Dispose()
        {
            dataLayer.Dispose();
        }

        /// <summary>
        /// Get a queryable entity collection
        /// </summary>
        public virtual IQueryable<TModel> Get
        {
            get { return dataLayer.Get; }
        }

        /// <summary>
        /// Find an object by key lookup
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual TModel Find(params object[] keys)
        {
            return dataLayer.Find(keys);
        }

        public virtual TModel Update(TModel model)
        {
            return dataLayer.Update(model);
        }

        public virtual TModel Add(TModel model)
        {
            return dataLayer.Add(model);
        }

        public virtual void Remove(TModel model)
        {
            dataLayer.Remove(model);
        }

        public virtual TEntity UndoChanges<TEntity>(TEntity model) where TEntity : class
        {
            return dataLayer.UndoChanges(model);
        }
    }

}