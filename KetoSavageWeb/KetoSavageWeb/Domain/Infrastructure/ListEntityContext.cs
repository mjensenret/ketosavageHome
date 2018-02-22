using KetoSavageWeb.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KetoSavageWeb.Domain.Infrastructure
{
    public class ListEntityContext<TEntity> : ListEntityContext<TEntity, int>, IEntityContext<TEntity>
        where TEntity : class, IKeyedEntity<int>
    {
        public ListEntityContext() : base() { }
        public ListEntityContext(IEnumerable<TEntity> data) : base(data) { }

        protected override int newKey()
        {
            return this.mockData.Count == 0 ? 1 : this.mockData.Max(x => x.Id) + 1;
        }
    }

    public class ListEntityContextGuid<TEntity> : ListEntityContext<TEntity, Guid>
        where TEntity : class, IKeyedEntity<Guid>
    {
        public ListEntityContextGuid() : base() { }
        public ListEntityContextGuid(IEnumerable<TEntity> data) : base(data) { }

        protected override Guid newKey()
        {
            return Guid.NewGuid();
        }
    }

    public class ListEntityContext<TEntity, TKey> : IEntityContext<TEntity, TKey>
        where TEntity : class, IKeyedEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        protected List<TEntity> mockData;

        public ListEntityContext()
        {
            this.mockData = new List<TEntity>();
        }

        public ListEntityContext(IEnumerable<TEntity> data)
            : this()
        {
            this.mockData.AddRange(data);
        }

        public IQueryable<TEntity> Get
        {
            get { return mockData.AsQueryable(); }
        }

        public Task<TEntity> FindAsync(TKey key)
        {
            return Task.FromResult(this.mockData.FirstOrDefault(m => m.Id.Equals(key)));
        }

        public Task CreateAsync(TEntity entity)
        {
            entity.Id = newKey();
            this.mockData.Add(entity);
            return Task.FromResult(0);
        }

        public Task UpdateAsync(TEntity entity)
        {
            int idx = this.mockData.IndexOf(this.mockData.FirstOrDefault(m => m.Id.Equals(entity.Id)));
            this.mockData[idx] = entity;
            return Task.FromResult(0);
        }

        public Task DeleteAsync(TEntity entity)
        {
            int idx = this.mockData.IndexOf(this.mockData.FirstOrDefault(m => m.Id.Equals(entity.Id)));
            this.mockData.RemoveAt(idx);
            return Task.FromResult(0);
        }

        public void Dispose()
        {
        }

        public bool SupportsTransactions
        {
            get { return false; }
        }

        public void BeginTransaction() { }

        public void CommitTransaction() { }

        public void RollbackTransaction() { }

        protected virtual TKey newKey()
        {
            return default(TKey);
        }
    }

    /// <summary>
    /// An implimentation of IEntityContext for use with mock data.
    /// Only implements Get.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SimpleListEntityContext<TEntity> : IEntityContext<TEntity, int>
        where TEntity : class
    {
        protected List<TEntity> mockData;

        public SimpleListEntityContext(IEnumerable<TEntity> data)
        {
            this.mockData.AddRange(data);
        }

        public IQueryable<TEntity> Get
        {
            get { return mockData.AsQueryable(); }
        }

        public Task<TEntity> FindAsync(int key)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(TEntity entity)
        {
            this.mockData.Add(entity);
            return Task.FromResult(0);
        }

        public Task UpdateAsync(TEntity entity)
        {
            return Task.FromResult(0);
        }

        public Task DeleteAsync(TEntity entity)
        {
            this.mockData.Remove(entity);
            return Task.FromResult(0);
        }

        public void Dispose()
        {
        }

        public bool SupportsTransactions
        {
            get { return false; }
        }

        public void BeginTransaction() { }

        public void CommitTransaction() { }

        public void RollbackTransaction() { }

    }
}