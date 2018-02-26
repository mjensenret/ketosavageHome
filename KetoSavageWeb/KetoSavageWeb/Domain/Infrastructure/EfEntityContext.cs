using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace KetoSavageWeb.Domain.Infrastructure
{
    public class EfEntityContext<TEntity> : IEntityContext<TEntity>
        where TEntity : class
    {
        protected DbContext context;
        private DbContextTransaction transaction;

        public EfEntityContext(DbContext context)
        {
            this.context = context;
            this.transaction = null;
        }

        public IQueryable<TEntity> Get
        {
            get { return this.dbSet; }
        }

        public Task<TEntity> FindAsync(int Id)
        {
            return this.dbSet.FindAsync(Id);
        }

        public Task CreateAsync(TEntity entity)
        {
            this.dbSet.Add(entity);
            return saveChangesAsync();
        }

        public Task UpdateAsync(TEntity entity)
        {
            if (this.context.Entry(entity).State == EntityState.Detached)
                attach(entity);
            return saveChangesAsync();
        }

        public Task DeleteAsync(TEntity entity)
        {
            this.dbSet.Remove(entity);
            return saveChangesAsync();
        }

        private Task saveChangesAsync()
        {
            try
            {
                return this.context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Exception baseEx = ex;
                while (baseEx.InnerException != null)
                    baseEx = baseEx.InnerException;

                if (baseEx.Message.Contains("Cannot insert duplicate key"))
                    throw new ApplicationException("This record already exists");
                else
                    throw new ApplicationException(baseEx.Message, baseEx);
            }
        }

        public T UndoChanges<T>(T entity) where T : class
        {
            var entry = context.Entry(entity);
            if (entry.State == EntityState.Modified)
            {
                entry.CurrentValues.SetValues(entry.OriginalValues);
                entry.State = EntityState.Unchanged;
            }
            else if (entry.State == EntityState.Added)
            {
                this.context.Set<T>().Remove(entity);
            }
            return entity;
        }

        public bool IsTracked(TEntity entity)
        {
            return context.Entry(entity).State != EntityState.Detached;
        }

        public bool SupportsTransactions
        {
            get { return true; }
        }

        public void BeginTransaction()
        {
            if (this.transaction != null)
                throw new ApplicationException("EfEntityContext.BeginTransaction called more than once");
            this.transaction = this.context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (this.transaction == null)
                throw new ApplicationException("EfEntityContext.CommitTransaction called with no open transaction");

            try
            {
                this.transaction.Commit();
                this.transaction.Dispose();
            }
            finally
            {
                this.transaction = null;
            }
        }

        public void RollbackTransaction()
        {
            if (this.transaction == null)
                throw new ApplicationException("DbDataLayer.RollbackTransaction called with no open transaction");

            try
            {
                this.transaction.Rollback();
                this.transaction.Dispose();
            }
            finally
            {
                this.transaction = null;
            }
        }

        public void Dispose()
        {
            if (this.transaction != null)
                this.RollbackTransaction();
            this.context.Dispose();
        }

        #region Implementation

        private DbSet<TEntity> dbSet { get { return this.context.Set<TEntity>(); } }

        private void attach(TEntity model)
        {
            // Get list of properties
            var properties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite);

            // Get list of properties where EditableAttribute.AllowEdit == false
            var readOnlyProperties = properties.Where(
                p =>
                    p.GetCustomAttributes(typeof(EditableAttribute), true).Where(a => ((EditableAttribute)a).AllowEdit == false).Any() ||
                    p.GetCustomAttributes(typeof(NotMappedAttribute), true).Any()
                ).ToList();

            // Check for MetadataType attribute
            MetadataTypeAttribute ma = model.GetType().GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>().FirstOrDefault();
            if (ma != null)
            {
                // Check Metadata Type for readonly properties
                var metaProperties = ma.MetadataClassType.GetProperties();
                var propNames = metaProperties.Where(
                    p =>
                        p.GetCustomAttributes(typeof(EditableAttribute), true).Where(a => ((EditableAttribute)a).AllowEdit == false).Any() ||
                        p.GetCustomAttributes(typeof(NotMappedAttribute), true).Any()
                        ).Select(p => p.Name).ToList();
                readOnlyProperties.AddRange(
                    model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => propNames.Contains(p.Name))
                    );
            }

            // Mark whole entity as Modified, when collection of readonly properties is empty
            if (readOnlyProperties.Count() == 0)
            {
                this.context.Entry(model).State = EntityState.Modified;
                return;
            }

            // Attach entity to DbContext
            this.dbSet.Attach(model);

            var propertiesForUpdate = properties.Except(readOnlyProperties);
            foreach (var propertyInfo in propertiesForUpdate)
            {
                // Skip navigation properties
                // Note: I don't think this will catch complex types. We need to put in a test for complex types or find another way
                //       to skip navigation properties.
                Type t = propertyInfo.PropertyType;
                if (t.IsPrimitive || t.IsValueType || t == typeof(string))
                {
                    this.context.Entry(model).Property(propertyInfo.Name).IsModified = true;
                }
            }
        }

        #endregion

    }
}