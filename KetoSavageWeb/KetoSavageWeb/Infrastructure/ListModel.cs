using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KetoSavageWeb.Infrastructure
{
    public class UserManagedListModel<TEntity> : ListModel<TEntity> where TEntity : class
    {
        public bool ShowInactive { get; set; }
        public override void SaveState(WriteCookieDelegate writeCookie)
        {
            writeCookie("ShowInactive", this.ShowInactive.ToString());
            base.SaveState(writeCookie);
        }
    }

    public class ListModel<T> : IEnumerable<T> where T : class
    {
        public delegate void WriteCookieDelegate(string name, string value, DateTime? expires = null);
        public IEnumerable<T> Items { get; set; }
        public int Page { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int MaxLinks { get; set; }
        public string Sort { get; set; }
        public bool Desc { get; set; }

        private string _search;
        public string Search
        {
            get { return string.IsNullOrWhiteSpace(_search) ? null : _search.Trim(); }
            set { _search = value; }
        }
        public string ActionName { get; set; }
        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
        }

        public ListModel()
        {
            //set defaults
            //TODO: switch to project settings
            Page = 1;
            ItemsPerPage = 10;
            MaxLinks = 1000;
            ActionName = "Index";
            Sort = null;
            Desc = false;
        }

        public void SetQuery(IOrderedQueryable<T> query)
        {
            this.TotalItems = query.Count();
        }
        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public virtual void SaveState(WriteCookieDelegate writeCookie)
        {
            int page = this.Page > this.TotalPages ? this.TotalPages : this.Page;
            writeCookie("Page", page.ToString());
            writeCookie("Sort", this.Sort);
            writeCookie("Desc", this.Desc.ToString());
            writeCookie("Search", this.Search);
        }
    }
    public static class PagingInfoHelper
    {
        /// <summary>
        /// Modify a sorted query to get a single page of results.
        /// </summary>
        /// <typeparam name="T">inferred from query</typeparam>
        /// <param name="query"></param>
        /// <param name="page">page number to return</param>
        /// <param name="size">results per page</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> Page<T>(this IOrderedQueryable<T> query, int page, int size, int totalItems)
        {
            var totalPages = (int)Math.Ceiling((decimal)totalItems / size);
            if (page > totalPages)
                page = totalPages;
            int skipCount = (page - 1) * size;
            if (skipCount > 0) query = (IOrderedQueryable<T>)query.Skip(skipCount);
            return (IOrderedQueryable<T>)query.Take(size);
        }
    }
}