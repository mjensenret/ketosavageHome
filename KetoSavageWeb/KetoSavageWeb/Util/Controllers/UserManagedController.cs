using KetoSavageWeb.Domain.Models;
using KetoSavageWeb.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KetoSavageWeb.Util.Controllers
{
    public enum ViewAction
    {
        Create,
        Edit,
        Details,
        Delete
    }

    public abstract class UserManagedController<TEntity, TRepository> : UserManagedController<TEntity, TRepository, TEntity>
        where TEntity : class, IUserManaged, IKeyedEntity<int>, IHasIsNew, new()
        where TRepository : UserManagedRepository<TEntity>
    {
        protected UserManagedController(TRepository r) : base(r) { }
        protected UserManagedController(TRepository r, Func<TEntity, string> userKeyField) : base(r, userKeyField) { }

        protected override TEntity createViewModel(TEntity entity)
        {
            return entity;
        }
    }

    public abstract class UserManagedController<TEntity, TRepository, TViewModel> : Controller
        where TEntity : class, IUserManaged, IKeyedEntity<int>, new()
        where TViewModel : IKeyedEntity<int>, IHasIsNew
        where TRepository : UserManagedRepository<TEntity>
    {
        protected TRepository Repository { get; private set; }

        /// <summary>
        /// Specify what field is displayed to the user to identify the entity (e.g. Name)
        /// </summary>
        protected Func<TEntity, string> UserKeyField { get; set; }

        protected UserManagedController(TRepository r) : this(r, null) { }

        /// <param name="userKeyField">Specify what field is displayed to the user to identify the entity (e.g. Name)</param>
        protected UserManagedController(TRepository r, Func<TEntity, string> userKeyField)
        {
            this.Repository = r;
            this.UserKeyField = userKeyField;
        }

        //
        // GET: /UserManaged/Details/5

        public virtual async Task<ActionResult> Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            TEntity entity = await this.Repository.FindAsync(id.Value);
            if (entity == null) return HttpNotFound();
            return createView(createViewModel(entity), ViewAction.Details, "Details");
        }

        //
        // GET: /UserManaged/Create

        public virtual ActionResult Create()
        {
            TEntity entity = new TEntity();
            return createView(createViewModel(entity), ViewAction.Create);
        }

        //
        // GET: /UserManaged/Edit/5

        public virtual async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            TEntity entity = await this.Repository.FindAsync(id.Value);
            if (entity == null) return HttpNotFound();

            return createView(createViewModel(entity), ViewAction.Edit);
        }

        //
        // POST: /UserManaged/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Edit(TViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TEntity entity;
                    if (model.IsNew)
                    {
                        entity = new TEntity();
                        updateEntity(entity, model);
                        entity = await Repository.CreateAsync(entity, User.Identity.Name);
                    }
                    else
                    {
                        entity = await Repository.FindAsync(model.Id);
                        updateEntity(entity, model);
                        entity = await Repository.UpdateAsync(entity, User.Identity.Name);
                    }

                    if (this.UserKeyField != null)
                        TempData["Message"] = string.Format("{0} has been saved", this.UserKeyField(entity));

                    return RedirectAfterSave(entity);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return createView(model, model.IsNew ? ViewAction.Create : ViewAction.Edit);
        }

        //
        // GET: /UserManaged/Delete/5

        public virtual async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            TEntity entity = await this.Repository.FindAsync(id.Value);
            if (entity == null) return HttpNotFound();

            return createView(createViewModel(entity), ViewAction.Delete, "Delete");
        }

        //
        // POST: /UserManaged/Delete/5

        [HttpPost, ActionName("Delete")]
        public virtual async Task<ActionResult> DeleteConfirmed(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            TEntity entity = await Repository.FindAsync(id.Value);
            if (entity == null) return HttpNotFound();

            try
            {
                await Repository.DeleteAsync(entity, User.Identity.Name);

                if (this.UserKeyField != null)
                    TempData["Message"] = string.Format("{0} has been deleted", this.UserKeyField(entity));

                return RedirectAfterDelete(entity);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return createView(createViewModel(entity), ViewAction.Delete, "Delete");
        }

        /// <summary>
        /// Called to create an instance of a TViewModel from TEntity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected abstract TViewModel createViewModel(TEntity entity);

        /// <summary>
        /// Called to update an entity from the view model before save.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        protected abstract void updateEntity(TEntity entity, TViewModel model);

        /// <summary>
        /// Called before returning an edit or detail view to configure the view state.
        /// Override to setup viewbag or modify model for display.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="forEdit"></param>
        /// <returns></returns>
        protected virtual ActionResult createView(TViewModel model, ViewAction action, string viewName = "Edit")
        {
            ViewBag.Action = action.ToString();
            ViewBag.ViewAction = action;

            return View(viewName, model);
        }

        /// <summary>
        /// Override to chane where user is redirected after save.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual ActionResult RedirectAfterSave(TEntity entity)
        {
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Override to chane where user is redirected after delete.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual ActionResult RedirectAfterDelete(TEntity entity)
        {
            return RedirectToAction("Index");
        }
    }

}