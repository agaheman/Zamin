﻿using Dapper;
using Microsoft.Data.SqlClient; using System.Data; using System.Linq.Expressions; using System.Reflection; using Zamin.Core.Contracts.Eve.Data.Commands;
using Zamin.Core.Domain.Eve.Entities;
using TypeExtensions = Zamin.Utilities.Eve.Extensions.TypeExtensions;
 namespace Zamin.Infra.Data.Sql.Commands.Eve {     public class EveBaseCommandRepository<TEntity, TDbContext> : IEveCommandRepository<TEntity>            where TEntity : EveBaseEntity, IEveIdentifiable            where TDbContext : EveBaseCommandDbContext     {         protected readonly TDbContext _dbContext;         protected readonly DbSet<TEntity> _commandDbSet;          public EveBaseCommandRepository(TDbContext dbContext)         {             _dbContext = dbContext;             _commandDbSet = dbContext.Set<TEntity>();         }          #region Public Method          #region Normal Method          #region Add          /// <summary>         /// Adds an object with type of <see cref="TEntity"></see>         /// </summary>         /// <param name="entity">Entity to be add</param>         /// <returns> <see cref="TEntity"/> Added entity</returns>         public TEntity Add(TEntity entity)         {             var context = _dbContext;             context.Add(entity);             var result = context.SaveChanges();              return entity;         }          /// <summary>         /// Adds set of objects with type of <see cref="IEnumerable{TEntity}"></see>         /// </summary>         /// <param name="entities">Entities to be add</param>         /// <returns><see cref="List{TEntity}"/>Added entites</returns>         public List<TEntity> Add(IEnumerable<TEntity> entities)         {             if (entities.Any())             {                 foreach (var entity in entities)                 {                     _dbContext.Add(entity);                 }                 _dbContext.SaveChanges();             }             return entities.ToList();         }          #endregion Add          #region Update          /// <summary>         /// Updates the input object with type of <see cref="TEntity"></see>         /// </summary>         /// <param name="entity">Entity to be updated</param>         /// <returns><see cref="object"/>The number of state entries written to the database</returns>         public int Update(TEntity entity)         {             if (_dbContext.Entry(entity).State == EntityState.Detached)             {                 var oldEntity = _commandDbSet.FirstOrDefault(item => item.Id == entity.Id);                 if (oldEntity != null)                     UpdateEntity(oldEntity, entity);             }              var result = _dbContext.SaveChanges();              return result;         }          public int Update(IEnumerable<TEntity> entities)         {             foreach (var entity in entities)             {                 if (_dbContext.Entry(entity).State == EntityState.Detached)                 {                     var oldEntity = _commandDbSet.FirstOrDefault(item => item.Id == entity.Id);                     if (oldEntity != null)                         UpdateEntity(oldEntity, entity);                 }             }              var result = _dbContext.SaveChanges();              return result;         }          /// <summary>         /// Updates the input object with type of <see cref="TEntity"></see>         /// </summary>         /// <param name="entities">Entities intended to be updated</param>         /// <param name="change">Properties to be updated</param>         /// <returns><see cref="object"/>The number of state entries written to the database</returns>         public int Update(IEnumerable<TEntity> entities, Action<TEntity> change)         {             var result = 0;              if (entities.Any())             {                 foreach (var entity in entities)                     change(entity);                  result = Update(entities);             }             return result;         }          /// <summary>         /// Updates the input object with type of <see cref="TEntity"></see>         /// </summary>         /// <param name="predicate">A function to test each element for a condition.</param>         /// <param name="change">Properties to be updated</param>         /// <returns><see cref="object"/>The number of state entries written to the database</returns>         public int Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> change)         {             var items = _commandDbSet.Where(predicate);             var result = Update(items, change);             return result;         }          #endregion Update          #region Delete          /// <summary>         /// Deletes the object By entity         /// </summary>         /// <summary xml:lang="fa">         /// حذف موجودیت بر اساس خود موجودیت         /// </summary>         /// <typeparam name="entity">Type of entity</typeparam>         /// <param name="entity"></param>         public int Delete(TEntity entity)         {             var deletedItemNumber = 0;              if (entity != null)             {                 _dbContext.Remove(entity);                 deletedItemNumber = _dbContext.SaveChanges();             }              return deletedItemNumber;         }          /// <summary>         /// Deletes the object By its Id         /// </summary>         /// <summary xml:lang="fa">         /// حذف موجودیت بر اساس مشخصه آن         /// </summary>         /// <typeparam name="long">Type of Id</typeparam>         /// <param name="id"></param>         public int Delete(long id)         {             var removeCandidate = _commandDbSet.FirstOrDefault(item => item.Id == id);             return Delete(removeCandidate);         }          /// <summary>         /// Deletes list of entities         /// </summary>         /// <summary xml:lang="fa">         /// حذف لیستی از موجودیت ها         /// </summary>         /// <typeparam name="entities">List of entities</typeparam>         /// <param name="entities"></param>         public int Delete(IEnumerable<TEntity> entities)         {             var deletedItems = 0;             if (entities.Any())             {                 foreach (var entity in entities)                 {                     _dbContext.Remove(entity);                 }                 deletedItems = _dbContext.SaveChanges();             }             return deletedItems;         }          /// <summary>         /// Deletes list of entities by their id         /// </summary>         /// <summary xml:lang="fa">         /// حذف لیستی از موجودیت ها بااستفاده از مشخصه های آنها         /// </summary>         /// <typeparam name="ids">List of ids</typeparam>         /// <param name="ids"></param>         public int Delete(IEnumerable<long> ids)         {             var context = _dbContext;             var items = _commandDbSet.Where(entity => ids.Contains(entity.Id)).ToList();             return Delete(items);         }          /// <summary>         /// Deletes list of entities by predicate         /// </summary>         /// <summary xml:lang="fa">         /// حذف لیستی از موجودیت ها بااستفاده از شرط         /// </summary>         /// <typeparam name="predicate">Where expression</typeparam>         /// <param name="predicate">A function to test each element for a condition.</param>         public int Delete(Expression<Func<TEntity, bool>> predicate)         {             var context = _dbContext;             var items = _commandDbSet.Where(predicate);             return Delete(items);         }          #endregion Delete          #endregion Normal Method          #region Async Method          #region Add          /// <summary>         /// Adds an object with type of <see cref="TEntity"></see>         /// </summary>         /// <param name="entity">Entity to be add</param>         /// <returns> <see cref="TEntity"/> Added entity</returns>         public Task<TEntity> AddAsync(TEntity entity)         {             var taskResult = Task.Run(() => Add(entity));             return taskResult;         }          /// <summary>         /// Adds set of objects with type of <see cref="List<TEntity>"></see>         /// </summary>         /// <param name="entities">Entities to be add</param>         /// <returns><see cref="List{TEntity}"/>Added entites</returns>         public Task<List<TEntity>> AddAsync(IEnumerable<TEntity> entities)         {             var taskResult = Task.Run(() => Add(entities));             return taskResult;         }          #endregion Add          #region Update          /// <summary>         /// Updates the input object with type of <see cref="TEntity"></see>         /// </summary>         /// <param name="entity">Entity to be updated</param>         /// <returns><see cref="object"/>The number of state entries written to the database</returns>         public Task<int> UpdateAsync(TEntity entity)         {             var taskResult = Task.Run(() => Update(entity));             return taskResult;         }          /// <summary>         /// Updates the input object with type of <see cref="TEntity"></see>         /// </summary>         /// <param name="entities">Entities intended to be updated</param>         /// <returns><see cref="object"/>The number of state entries written to the database</returns>         public Task<int> UpdateAsync(IEnumerable<TEntity> entities)         {             var taskResult = Task.Run(() => Update(entities));             return taskResult;         }          /// <summary>         /// Updates the input object with type of <see cref="TEntity"></see>         /// </summary>         /// <param name="entities">Entities intended to be updated</param>         /// <param name="change">Properties to be updated</param>         /// <returns><see cref="object"/>The number of state entries written to the database</returns>         public Task<int> UpdateAsync(IEnumerable<TEntity> entities, Action<TEntity> change)         {             var taskResult = Task.Run(() => Update(entities, change));             return taskResult;         }          /// <summary>         /// Updates the input object with type of <see cref="TEntity"></see>         /// </summary>         /// <param name="predicate">A function to test each element for a condition.</param>         /// <param name="change">Properties to be updated</param>         /// <returns><see cref="object"/>The number of state entries written to the database</returns>         public Task<int> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> change)         {             var taskResult = Task.Run(() => Update(predicate, change));             return taskResult;         }          #endregion Update          #region Delete          /// <summary>         /// Deletes the object By its Id         /// </summary>         /// <summary xml:lang="fa">         /// حذف موجودیت بر اساس مشخصه آن         /// </summary>         /// <typeparam name="long">Type of Id</typeparam>         /// <param name="id"></param>         public Task<int> DeleteAsync(long id)         {             var taskResult = Task.Run(() => Delete(id));             return taskResult;         }          /// <summary>         /// Deletes the object By entity         /// </summary>         /// <summary xml:lang="fa">         /// حذف موجودیت بر اساس خود موجودیت         /// </summary>         /// <typeparam name="entity">Type of entity</typeparam>         /// <param name="entity"></param>         public Task<int> DeleteAsync(TEntity entity)         {             var taskResult = Task.Run(() => Delete(entity));             return taskResult;         }          /// <summary>         /// Deletes list of entities         /// </summary>         /// <summary xml:lang="fa">         /// حذف لیستی از موجودیت ها         /// </summary>         /// <typeparam name="entities">List of entities</typeparam>         /// <param name="entities"></param>         public Task<int> DeleteAsync(IEnumerable<TEntity> entities)         {             var taskResult = Task.Run(() => Delete(entities));             return taskResult;         }          /// <summary>         /// Deletes list of entities by their id         /// </summary>         /// <summary xml:lang="fa">         /// حذف لیستی از موجودیت ها بااستفاده از مشخصه های آنها         /// </summary>         /// <typeparam name="ids">List of ids</typeparam>         /// <param name="ids"></param>         public Task<int> DeleteAsync(IEnumerable<long> ids)         {             var taskResult = Task.Run(() => Delete(ids));             return taskResult;         }          /// <summary>         /// Deletes list of entities by predicate         /// </summary>         /// <summary xml:lang="fa">         /// حذف لیستی از موجودیت ها بااستفاده از شرط         /// </summary>         /// <typeparam name="predicate">Where expression</typeparam>         /// <param name="predicate">A function to test each element for a condition.</param>         public Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)         {             var taskResult = Task.Run(() => Delete(predicate));             return taskResult;         }          #endregion Delete          protected IQueryable<TEntity> GetDbSet(bool readUncommitted = true)         {             return _commandDbSet;         }          #endregion Async Method          #endregion Public Method          #region Private Method          private void UpdateEntity(TEntity prev, TEntity current)         {             var validPropsName = _dbContext.Entry(prev).Metadata.GetProperties().Select(x => x.Name);             foreach (PropertyInfo prop in typeof(TEntity).GetProperties()                 .Where(x => validPropsName.Contains(x.Name)))             {                 var currentVal = prop.GetValue(current, null);                 if (prop.CanWrite)                     prop.SetValue(prev, currentVal);             }         }          #endregion Private Method          #region Transaction          //          #endregion Transaction          #region Stored Procedure          protected List<T> StoredProcedureQuery<T>(string spName, List<EveStoredProcedureParam> spParams = null) where T : class, new()         {             if (spName.Contains(Environment.NewLine) || spName.Contains(' '))                 return new List<T>();              using IDbConnection db = new SqlConnection(_dbContext.Database.GetDbConnection().ConnectionString);             try             {                 db.Open();                 var spCallParams = new DynamicParameters();                  if (spParams != null)                     foreach (var param in spParams)                     {                         if (param.Value != null)                             spCallParams.Add(param.Name, TypeExtensions.ChangeToNotNullType(param.Value, param.Type));                     }                  var spResult = db.Query<T>(spName, spCallParams, commandType: CommandType.StoredProcedure).ToList();                  return spResult;             }             finally             {                 db.Close();             }         }          protected T StoredProcedureQueryFirstOrDefault<T>(string spName, List<EveStoredProcedureParam> spParams = null) where T : class, new()         {             if (spName.Contains(Environment.NewLine) || spName.Contains(" "))                 return default;              using IDbConnection db = new SqlConnection(_dbContext.Database.GetDbConnection().ConnectionString);             try             {                 db.Open();                 var spCallParams = new DynamicParameters();                  if (spParams != null)                     foreach (var param in spParams)                     {                         if (param.Value != null)                             spCallParams.Add(param.Name, TypeExtensions.ChangeToNotNullType(param.Value, param.Type));                     }                  var spResult = db.QueryFirstOrDefault<T>(spName, spCallParams, commandType: CommandType.StoredProcedure);                  return spResult;             }             finally             {                 db.Close();             }         }          protected int StoredProcedureExecute(string spName, List<EveStoredProcedureParam> spParams = null)         {             if (spName.Contains(Environment.NewLine) || spName.Contains(" "))                 return 0;              using IDbConnection db = new SqlConnection(_dbContext.Database.GetDbConnection().ConnectionString);             try             {                 db.Open();                 var spCallParams = new DynamicParameters();                  if (spParams != null)                     foreach (var param in spParams)                     {                         if (param.Value != null)                             spCallParams.Add(param.Name, TypeExtensions.ChangeToNotNullType(param.Value, param.Type));                     }                  var spResult = db.Execute(spName, spCallParams, commandType: CommandType.StoredProcedure);                  return spResult;             }             finally             {                 db.Close();             }         }          protected T StoredProcedureExecuteScalar<T>(string spName, List<EveStoredProcedureParam> spParams = null)         {             if (spName.Contains(Environment.NewLine) || spName.Contains(" "))                 return default;              using IDbConnection db = new SqlConnection(_dbContext.Database.GetDbConnection().ConnectionString);             try             {                 db.Open();                 var spCallParams = new DynamicParameters();                  if (spParams != null)                     foreach (var param in spParams)                     {                         if (param.Value != null)                             spCallParams.Add(param.Name, TypeExtensions.ChangeToNotNullType(param.Value, param.Type));                     }                  var spResult = db.ExecuteScalar<T>(spName, spCallParams, commandType: CommandType.StoredProcedure);                  return spResult;             }             finally             {                 db.Close();             }         }          #endregion Stored Procedure          #region Raw Sql Query          protected List<T> RawQuery<T>(string query, bool readUnCommitted = true)         {             //set target context             using IDbConnection db = new SqlConnection(_dbContext.Database.GetDbConnection().ConnectionString);             try             {                 db.Open();                  //  IDbTransaction dbTransaction = db.BeginTransaction(readUnCommitted ? IsolationLevel.ReadUncommitted : IsolationLevel.ReadCommitted);                 //var result = db.Query<T>(query, transaction: dbTransaction).ToList();                 var result = db.Query<T>(query).ToList();                 return result;             }             finally             {                 db.Close();             }         }          protected T RawQueryFirstOrDefault<T>(string query, bool readUnCommitted = true) where T : class, new()         {             using IDbConnection db = new SqlConnection(_dbContext.Database.GetDbConnection().ConnectionString);             try             {                 db.Open();                 var result = db.QueryFirstOrDefault<T>(query);                 return result;             }             finally             {                 db.Close();             }         }          protected int RawExecute(string query, bool readUnCommitted = true)         {             //set target context             using IDbConnection db = new SqlConnection(_dbContext.Database.GetDbConnection().ConnectionString);             try             {                 db.Open();                  //  IDbTransaction dbTransaction = db.BeginTransaction(readUnCommitted ? IsolationLevel.ReadUncommitted : IsolationLevel.ReadCommitted);                 //var result = db.Query<T>(query, transaction: dbTransaction).ToList();                 var result = db.Execute(query);                 return result;             }             finally             {                 db.Close();             }         }          protected T RawExecuteScalar<T>(string query, bool readUnCommitted = true, bool isQuery = true)         {             //set target context             using IDbConnection db = new SqlConnection(_dbContext.Database.GetDbConnection().ConnectionString);             try             {                 db.Open();                  //  IDbTransaction dbTransaction = db.BeginTransaction(readUnCommitted ? IsolationLevel.ReadUncommitted : IsolationLevel.ReadCommitted);                 //var result = db.Query<T>(query, transaction: dbTransaction).ToList();                 var result = db.ExecuteScalar<T>(query);                 return result;             }             finally             {                 db.Close();             }         }          #endregion Raw Sql Query          protected TResult ExecuteTransactional<TInput, TResult>(IQueryable<TInput> query, Func<IQueryable<TInput>, TResult> queryAction, bool readUncommitted, bool isQuery = true)         {             //set target context              //If there is already an open transaction             if (_dbContext.Database.CurrentTransaction != null)                 return queryAction(query);              var isolationLevel = readUncommitted ? IsolationLevel.ReadUncommitted : IsolationLevel.ReadCommitted;              using var transaction = _dbContext.Database.BeginTransaction(isolationLevel);             //The explicit call to dbContextTransaction.Rollback() is  unnecessary, because disposing the transaction at the end of the using block will take care of rolling back.             //If an error of sufficient severity occurs in SQL Server, the transaction will get automatically rolled back, and the call to dbContextTransaction.Rollback() in the catch block will actually fail.             var result = queryAction(query);             transaction.Commit();             return result;         }      } }