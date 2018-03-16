using KetoSavageWeb.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb.Infrastructure
{
    public class SelectListData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public static class UserManagedExtensions
    {
        /// <summary>
        /// Create a SelectList from an IQueryable of UserManaged data.
        /// Be sure the query does not filter on Deleted or Active (i.e. use on GetAll).
        /// Deleted and Active filters will be added to the query.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="allDataQuery"></param>
        /// <param name="selectedId">
        /// Currently selected item in the select list.
        /// Note: If currently selected ID is Deleted or not Active, the record will be included in the list and will be prepended with an asterisk (*).
        /// </param>
        /// <param name="listData">
        /// A function that takes a UserManaged and returns a SelectListData.
        /// Example:  m => new SelectListData { Id = m.Id, Name = m.Name, IsActive = m.IsActive, IsDeleted = m.IsDeleted }
        /// </param>
        /// <returns></returns>
        /// <example>
        ///   ViewBag.CarrierId = carrierRepository.GetAll.ToSelectList(
        ///      model.CarrierId,
        ///      m => new SelectListData { Id = m.Id, Name = m.Name, IsActive = m.IsActive, IsDeleted = m.IsDeleted }
        ///      );
        /// </example>
        /// <remarks>
        /// This code should be modified and moved into shared code library.
        /// UserManaged could be replaced with something like IHasIsDeleted, IHasIsActive.
        /// </remarks>
        /// 
        public static SelectList ToSelectList<TModel>(this IQueryable<TModel> allDataQuery, int? selectedId, Func<TModel, SelectListData> listData)
            where TModel : UserManaged
        {
            IQueryable<TModel> q1;

            if (selectedId.HasValue)
                q1 = allDataQuery.Where(m => (m.IsDeleted == false && m.IsActive == true) || m.Id == selectedId);
            else
                q1 = allDataQuery.Where(m => m.IsDeleted == false && m.IsActive == true);

            var q2 = q1
                .Select(listData)
                .ToList()
                .Select(i => new { i.Id, Name = string.Format("{1}{0}", i.Name, i.IsDeleted || !i.IsActive ? "*" : "") })
                .OrderBy(li => li.Name);

            return new SelectList(q2, "Id", "Name", selectedId);
        }
    }
}