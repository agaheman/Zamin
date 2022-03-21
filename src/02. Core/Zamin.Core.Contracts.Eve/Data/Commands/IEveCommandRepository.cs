using System.Linq.Expressions;
using Zamin.Core.Domain.Eve.Entities;

namespace Zamin.Core.Contracts.Eve.Data.Commands
{
    /// <summary>
    /// This interface includes all methodes and properties needed for BaseCommandRepository
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity</typeparam>
    public interface IEveCommandRepository<TEntity> where TEntity : IEveIdentifiable
    {
        #region Normal

        #region Add

        /// <summary>
        /// Adds an object with type of <see cref="TEntity"></see>
        /// </summary>
        /// <param name="entity">Entity to be add</param>
        /// <returns> <see cref="TEntity"/> Added entity</returns>
        TEntity Add(TEntity entity);

        /// <summary>
        /// Adds set of objects with type of <see cref="IEnumerable{TEntity}"></see>
        /// </summary>
        /// <param name="entities">Entities to be add</param>
        /// <returns><see cref="List{TEntity}"/>Added entites</returns>
        List<TEntity> Add(IEnumerable<TEntity> entities);

        #endregion Add

        #region Update

        /// <summary>
        /// Updates the input object with type of <see cref="TEntity"></see>
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        /// <returns><see cref="object"/>The number of state entries written to the database</returns>
        int Update(TEntity entity);

        /// <summary>
        /// Updates the input object with type of <see cref="TEntity"></see>
        /// </summary>
        /// <param name="entities">Entities intended to be updated</param>
        /// <param name="change">Properties to be updated</param>
        /// <returns><see cref="object"/>The number of state entries written to the database</returns>
        int Update(IEnumerable<TEntity> entities, Action<TEntity> change);

        /// <summary>
        /// Updates the input object with type of <see cref="TEntity"></see>
        /// </summary>
        /// <param name="entities">Entities intended to be updated</param>
        /// <returns><see cref="object"/>The number of state entries written to the database</returns>
        int Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates the input object with type of <see cref="TEntity"></see>
        /// </summary>
        /// <param name="predicate">Predicate to select entities for being updated</param>
        /// <param name="change">Properties to be updated</param>
        /// <returns><see cref="object"/>The number of state entries written to the database</returns>
        int Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> change);

        #endregion Update

        #region Delete

        /// <summary>
        /// Deletes the object By its Id
        /// </summary>
        /// <summary xml:lang="fa">
        /// حذف موجودیت بر اساس مشخصه آن
        /// </summary>
        /// <typeparam name="long">Type of Id</typeparam>
        /// <param name="id"></param>
        int Delete(long id);

        /// <summary>
        /// Deletes list of entities by their id
        /// </summary>
        /// <summary xml:lang="fa">
        /// حذف لیستی از موجودیت ها بااستفاده از مشخصه های آنها
        /// </summary>
        /// <typeparam name="ids">List of ids</typeparam>
        /// <param name="ids"></param>
        int Delete(IEnumerable<long> ids);

        /// <summary>
        /// Deletes the object By entity
        /// </summary>
        /// <summary xml:lang="fa">
        /// حذف موجودیت بر اساس خود موجودیت
        /// </summary>
        /// <typeparam name="entity">Type of entity</typeparam>
        /// <param name="entity"></param>
        int Delete(TEntity entity);

        /// <summary>
        /// Deletes list of entities
        /// </summary>
        /// <summary xml:lang="fa">
        /// حذف لیستی از موجودیت ها
        /// </summary>
        /// <typeparam name="entities">List of entities</typeparam>
        /// <param name="entities"></param>
        int Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes list of entities by predicate
        /// </summary>
        /// <summary xml:lang="fa">
        /// حذف لیستی از موجودیت ها بااستفاده از شرط
        /// </summary>
        /// <typeparam name="predicate">Where expression</typeparam>
        /// <param name="predicate"></param>
        int Delete(Expression<Func<TEntity, bool>> predicate);

        #endregion Delete

        #endregion Normal

        #region Async

        #region Add

        /// <summary>
        /// Adds an object with type of <see cref="TEntity"></see>
        /// </summary>
        /// <param name="entity">Entity to be add</param>
        /// <returns> <see cref="TEntity"/> Added entity</returns>
        Task<TEntity> AddAsync(TEntity entity);

        /// <summary>
        /// Adds set of objects with type of <see cref="IEnumerable<TEntity>"></see>
        /// </summary>
        /// <param name="entities">Entities to be add</param>
        /// <returns><see cref="List{TEntity}"/>Added entites</returns>
        Task<List<TEntity>> AddAsync(IEnumerable<TEntity> entities);

        #endregion Add

        #region Update

        /// <summary>
        /// Updates the input object with type of <see cref="TEntity"></see>
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        /// <returns><see cref="object"/>The number of state entries written to the database</returns>
        Task<int> UpdateAsync(TEntity entity);

        /// <summary>
        /// Updates the input object with type of <see cref="TEntity"></see>
        /// </summary>
        /// <param name="entities">Entities intended to be updated</param>
        /// <returns><see cref="object"/>The number of state entries written to the database</returns>
        Task<int> UpdateAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates the input object with type of <see cref="TEntity"></see>
        /// </summary>
        /// <param name="entities">Entities intended to be updated</param>
        /// <param name="change">Properties to be updated</param>
        /// <returns><see cref="object"/>The number of state entries written to the database</returns>
        Task<int> UpdateAsync(IEnumerable<TEntity> entities, Action<TEntity> change);

        /// <summary>
        /// Updates the input object with type of <see cref="TEntity"></see>
        /// </summary>
        /// <param name="predicate">Predicate to select entities for being updated</param>
        /// <param name="change">Properties to be updated</param>
        /// <returns><see cref="object"/>The number of state entries written to the database</returns>
        Task<int> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> change);

        #endregion Update

        #region Delete

        /// <summary>
        /// Deletes the object By its Id
        /// </summary>
        /// <summary xml:lang="fa">
        /// حذف موجودیت بر اساس مشخصه آن
        /// </summary>
        /// <typeparam name="long">Type of Id</typeparam>
        /// <param name="id"></param>
        Task<int> DeleteAsync(long id);

        /// <summary>
        /// Deletes list of entities by their id
        /// </summary>
        /// <summary xml:lang="fa">
        /// حذف لیستی از موجودیت ها بااستفاده از مشخصه های آنها
        /// </summary>
        /// <typeparam name="ids">List of ids</typeparam>
        /// <param name="ids"></param>
        Task<int> DeleteAsync(IEnumerable<long> ids);

        /// <summary>
        /// Deletes the object By entity
        /// </summary>
        /// <summary xml:lang="fa">
        /// حذف موجودیت بر اساس خود موجودیت
        /// </summary>
        /// <typeparam name="entity">Type of entity</typeparam>
        /// <param name="entity"></param>
        Task<int> DeleteAsync(TEntity entity);

        /// <summary>
        /// Deletes list of entities
        /// </summary>
        /// <summary xml:lang="fa">
        /// حذف لیستی از موجودیت ها
        /// </summary>
        /// <typeparam name="entities">List of entities</typeparam>
        /// <param name="entities"></param>
        Task<int> DeleteAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes list of entities by predicate
        /// </summary>
        /// <summary xml:lang="fa">
        /// حذف لیستی از موجودیت ها بااستفاده از شرط
        /// </summary>
        /// <typeparam name="predicate">Where expression</typeparam>
        /// <param name="predicate"></param>
        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion Delete

        #endregion Async
    }
}