using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Zamin.Core.Contracts.Eve.Data.Queries;
using static Zamin.Core.Contracts.Eve.Data.Queries.EveQueryEnums;

namespace Zamin.Infra.Data.Sql.Queries.Eve
{
    public class EveBaseQueryRepository<TEntity, TDbContext> : IEveQueryRepository<TEntity>
            where TEntity : class
            where TDbContext : BaseQueryDbContext
    {
        protected readonly TDbContext _dbContext;
        protected readonly DbSet<TEntity> _queryDbSet;

        public EveBaseQueryRepository(TDbContext dbContext)
        {
            _dbContext = dbContext;
            _queryDbSet = dbContext.Set<TEntity>();

        }



        #region Normal Method


        #region GetAll
        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <summary xml:lang="fa">
        /// را بر می گرداند TEntity تمام موجودیت ها از نوع
        /// </summary>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        public IEnumerable<TEntity> GetAll(int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.Take(maxCount);

            if (orderByDictionary?.Count() > 0)
                query = query.OrderBy(SortGenerator.GetSortString(orderByDictionary));

            if (joins?.Count() > 0)
                query = query.ToJoined(joins);
            return ExecuteTransactional(query, c => c.ToList(), readUncommitted);
        }

        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <typeparam name="TResult">Type of return value items</typeparam>
        /// <param name="selector">A transform function to apply to each elemnt</param>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        public IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector, int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.Take(maxCount);

            if (orderByDictionary?.Count() > 0)
                query = query.OrderBy(SortGenerator.GetSortString(orderByDictionary));

            if (joins?.Count() > 0)
                query = query.ToJoined(joins);

            var transformedQuery = query.Select(selector);

            return ExecuteTransactional(transformedQuery, c => c.ToList(), readUncommitted);
        }

        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <param name="paging">Indicate result paging information</param>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        public EveOutputParam<TEntity, EveBasePagination> GetAll(EveBasePagination paging, int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.Take(maxCount);
            if (joins?.Count() > 0)
                query = query.ToJoined(joins);



            //Count total number of result if needed, befor adding ordering
            if (paging.NeedTotalCount)
                paging.TotalCount = ExecuteTransactional(query, q => q.Count(), readUncommitted);

            //Adding Paging
            query = query.AddPaging(paging);

            if (orderByDictionary?.Count() > 0)
                query = query.OrderBy(SortGenerator.GetSortString(orderByDictionary));

            var output = new EveOutputParam<TEntity, EveBasePagination>()
            {
                Result = ExecuteTransactional(query, c => c.ToList(), readUncommitted),
                Paging = paging
            };

            return output;
        }

        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <typeparam name="TResult">Type of return value items</typeparam>
        /// <param name="paging">Indicate result paging information</param>
        /// <param name="selector">A transform function to apply to each elemnt</param>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        public EveOutputParam<TResult, EveBasePagination> GetAll<TResult>(EveBasePagination paging, Expression<Func<TEntity, TResult>> selector, int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.Take(maxCount);
            if (orderByDictionary?.Count() > 0)
                query = query.OrderBy(SortGenerator.GetSortString(orderByDictionary));

            if (joins?.Count() > 0)
                query = query.ToJoined(joins);

            var transformedQuery = query.Select(selector);

            if (paging.NeedTotalCount)
                paging.TotalCount = ExecuteTransactional(transformedQuery, q => q.Count(), readUncommitted);

            //Adding Paging
            query = query.AddPaging(paging);

            var result = ExecuteTransactional(transformedQuery, c => c.ToList(), readUncommitted);
            var output = new EveOutputParam<TResult, EveBasePagination>()
            {
                Result = result,
                Paging = paging
            };
            return output;
        }
        #endregion

        #region Distinct
        /// <summary>
        /// Returns distinct elements from the <paramref name="TEntity"/> collection by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector">A transform function to apply to each element and compare for their equality</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="comparer">An IEqualityComparare<in <paramref name="TResult"/>> to campare  values.</param>
        /// <returns></returns>
        public List<TResult> Distinct<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, IEqualityComparer<TResult> comparer = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();

            if (predicate != null)
                q = q.Where(predicate);

            var resultQuery = q.Select(selector);

            resultQuery =
                comparer == null
                ? resultQuery.Distinct()
                : resultQuery.Distinct(comparer);

            return ExecuteTransactional(resultQuery, c => c.ToList(), readUncommitted);
        }

        public EveOutputParam<TResult, TPaging> Distinct<TResult, TPaging>(Expression<Func<TEntity, TResult>> selector, EveInputParam<TEntity, TPaging> input, IEqualityComparer<TResult> comparer = null, bool readUncommitted = true) where TPaging : EveBasePagination, new()

        {
            //Adding Conditions
            var query = input.FilterSet?.Filters?.Count > 0
                        ? input.FilterSet.ToSqlServerQueryable(_queryDbSet)
                        : _queryDbSet.AsQueryable();

            //Adding Joins
            if (input.JoinList?.Count > 0)
                query.ToJoined(input.JoinList);

            //Adding Projection
            var transformedQuery = query.Select(selector);

            //Preparing result
            var distinctQuery =
                comparer == null
                ? transformedQuery.Distinct()
                : transformedQuery.Distinct(comparer);

            //Adding Ordering
            //It's irrational, but Distinct().Count() cause error! it should be something between Distinct() and Count()
            if (input.OrderByDictionary?.Count == 0)
                input.OrderByDictionary.Add("Id", SortDirection.Asc);
            distinctQuery = distinctQuery.OrderBy(SortGenerator.GetSortString(input.OrderByDictionary));

            //Count total number of result if needed, befor adding ordering and paging
            if (input.Paging.NeedTotalCount)
                input.Paging.TotalCount = ExecuteTransactional(distinctQuery, q => q.Count(), readUncommitted);

            //Adding Paging
            distinctQuery = distinctQuery.AddPaging(input.Paging);

            var result = new EveOutputParam<TResult, TPaging>()
            {
                FilterSet = input.FilterSet,
                JoinList = input.JoinList,
                OrderByDictionary = input.OrderByDictionary,
                Paging = input.Paging,
                Result = ExecuteTransactional(distinctQuery, c => c.ToList(), readUncommitted)
            };

            return result;
        }


        #endregion

        #region First
        /// <summary>
        /// Returns the first element of the <paramref name="TEntity"/> collectio that satisfies a specefic condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns></returns>
        public TEntity First(Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();

            if (joins?.Count() > 0)
                q = q.ToJoined(joins);

            if (predicate != null)
                q = q.Where(predicate);

            //Adding Ordering
            if (orderByDictionary?.Count() > 0)
                q = q.OrderBy(SortGenerator.GetSortString(orderByDictionary));

            return ExecuteTransactional(q, c => c.First(), readUncommitted);
        }

        public TResult First<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();

            if (predicate != null)
                q = q.Where(predicate);

            //Adding Ordering
            if (orderByDictionary?.Count() > 0)
                q = q.OrderBy(SortGenerator.GetSortString(orderByDictionary));

            return ExecuteTransactional(q.Select(selector), c => c.First(), readUncommitted);
        }
        #endregion

        #region FirstOrDefault
        /// <summary>
        /// Returns the first element of the <paramref name="TEntity"/> collectio that satisfies a specefic condition or a default value if no such element is found.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns></returns>
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();

            if (joins?.Count() > 0)
                q = q.ToJoined(joins);

            if (predicate != null)
                q = q.Where(predicate);

            //Adding Ordering
            if (orderByDictionary?.Count() > 0)
                q = q.OrderBy(SortGenerator.GetSortString(orderByDictionary));

            return ExecuteTransactional(q, c => c.FirstOrDefault(), readUncommitted);
        }

        public TResult FirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();

            if (predicate != null)
                q = q.Where(predicate);

            //Adding Ordering
            if (orderByDictionary?.Count() > 0)
                q = q.OrderBy(SortGenerator.GetSortString(orderByDictionary));

            var transformedQuery = q.Select(selector);

            return ExecuteTransactional(transformedQuery, c => c.FirstOrDefault(), readUncommitted);
        }
        #endregion

        #region Find
        /// <summary>
        /// Search with lambda expression
        /// </summary>
        /// <typeparam name="Expression<Func<TEntity, bool>>">Type of Id</typeparam>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        public List<TEntity> Find(Expression<Func<TEntity, bool>> predicate, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            if (predicate != null)
            {
                //Adding the Condition
                var query = _queryDbSet.Where(predicate);

                //Adding Joins
                if (joins != null)
                    query = query.ToJoined(joins);

                //Adding Ordering
                if (orderByDictionary?.Count() > 0)
                    query = query.OrderBy(SortGenerator.GetSortString(orderByDictionary));

                return ExecuteTransactional(query, c => c.ToList(), readUncommitted);
            }

            return null;
        }

        /// <summary>
        /// Search with lambda expression
        /// </summary>
        /// <typeparam name="Expression<Func<TEntity, bool>>">Type of Id</typeparam>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="selector">A transform function to apply to each element</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        public List<TResult> Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            //TODO Check Ambiguity
            if (predicate != null)
            {
                //Adding the Condition
                var query = _queryDbSet.Where(predicate);

                //Adding Joins
                if (joins != null)
                    query = query.ToJoined(joins);

                //Adding Ordering
                if (orderByDictionary?.Count() > 0)
                    query = query.OrderBy(SortGenerator.GetSortString(orderByDictionary));

                //Adding Projection
                var transformedQuery = query.Select(selector);

                return ExecuteTransactional(transformedQuery, c => c.ToList(), readUncommitted);
            }

            return null;
        }

        /// <summary>
        /// Search with lambda expression
        /// </summary>
        /// <typeparam name="Expression<Func<TEntity, bool>>">Type of Id</typeparam>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="selector">A transform function to apply to each element</param>
        /// <param name="pagination">An object to determine pagination of the result</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        public EveOutputParam<TResult, TPaging> Find<TResult, TPaging>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, TPaging pagination, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true) where TPaging : EveBasePagination, new()
        {
            if (predicate != null)
            {
                //Adding the Condition
                var query = _queryDbSet.Where(predicate);

                //Adding Joins
                if (joins != null)
                    query = query.ToJoined(joins);

                //Adding Ordering
                if (orderByDictionary?.Count() > 0)
                    query = query.OrderBy(SortGenerator.GetSortString(orderByDictionary));

                //Adding Paging
                query = query.AddPaging(pagination);

                //Adding Projection
                var transformedQuery = query.Select(selector);

                var result = new EveOutputParam<TResult, TPaging>()
                {
                    Result = ExecuteTransactional(transformedQuery, c => c.ToList(), readUncommitted),
                    Paging = pagination
                };
                if (result.Paging.NeedTotalCount)
                {
                    result.Paging.TotalCount = Count(predicate);
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// Finds objects with type of <see cref="TEntity"></see> whit input type of <see cref="EveInputParam{TEntity, TPaging}"/>
        /// </summary>
        /// <typeparam name="TPaging">Type of Paging</typeparam>
        /// <param name="input"></param>
        /// <returns><see cref="EveOutputParam{TEntity, TPaging}"/></returns>
        public EveOutputParam<TEntity, TPaging> Find<TPaging>(EveInputParam<TEntity, TPaging> input, bool readUncommitted = true) where TPaging : EveBasePagination, new()
        {
            //Adding Conditions
            var query = input.FilterSet?.Filters?.Count > 0
                        ? input.FilterSet.ToSqlServerQueryable(_queryDbSet)
                        : _queryDbSet.AsQueryable();

            //Adding Joins
            if (input.JoinList?.Count > 0)
                query = query.ToJoined(input.JoinList);

            //Adding Ordering
            if (input.OrderByDictionary?.Count > 0)
                query = query.OrderBy(SortGenerator.GetSortString(input.OrderByDictionary));

            //Adding Paging
            query = query.AddPaging(input.Paging);

            //Preparing result
            var resultData = ExecuteTransactional(query, c => c.ToList(), readUncommitted);

            var result = new EveOutputParam<TEntity, TPaging>()
            {
                FilterSet = input.FilterSet,
                JoinList = input.JoinList,
                OrderByDictionary = input.OrderByDictionary,
                Paging = input.Paging,
                Result = resultData
            };

            if (result.Paging.NeedTotalCount)
            {
                result.Paging.TotalCount = Count(input.FilterSet);
            }

            return result;
        }

        /// <summary>
        /// Finds objects with type of <see cref="TEntity"></see> whit input type of <see cref="EveInputParam{TEntity, TPaging}"/>
        /// </summary>
        /// <typeparam name="TPaging">Type of Paging</typeparam>
        /// <param name="input"></param>
        /// <returns><see cref="EveOutputParam{TEntity, TPaging}"/></returns>
        public EveOutputParam<TResult, TPaging> Find<TResult, TPaging>(EveInputParam<TEntity, TPaging> input, Expression<Func<TEntity, TResult>> selector, bool readUncommitted = true) where TPaging : EveBasePagination, new()
        {
            //Adding Conditions
            var query = input.FilterSet?.Filters?.Count > 0
                        ? input.FilterSet.ToSqlServerQueryable(_queryDbSet)
                        : _queryDbSet.AsQueryable();

            //Adding Joins
            if (input.JoinList?.Count > 0)
                query.ToJoined(input.JoinList);


            //Adding Ordering
            if (input.OrderByDictionary?.Count > 0)
                query = query.OrderBy(SortGenerator.GetSortString(input.OrderByDictionary));

            //Adding Paging
            query = query.AddPaging(input.Paging);

            //Adding Projection
            var transformedQuery = query.Select(selector);

            //Preparing result
            var resultData = ExecuteTransactional(transformedQuery, c => c.ToList(), readUncommitted);

            var result = new EveOutputParam<TResult, TPaging>()
            {
                FilterSet = input.FilterSet,
                JoinList = input.JoinList,
                OrderByDictionary = input.OrderByDictionary,
                Paging = input.Paging,
                Result = resultData
            };

            if (result.Paging.NeedTotalCount)
            {
                result.Paging.TotalCount = Count(input.FilterSet);
            }

            return result;
        }

        #endregion

        #region Any
        public bool Any(Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();

            if (predicate == null)
                return _queryDbSet.Any();
            else
                return ExecuteTransactional(_queryDbSet, c => c.Any(predicate), readUncommitted);
        }
        #endregion

        #region Max

        /// <summary>
        /// Get maximum value of selector item
        /// </summary>
        /// <typeparam name="TField">Field to get maximum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <returns>Maximum value</returns>
        public TField Max<TField>(Expression<Func<TEntity, TField>> selector, bool readUncommitted = true)
        {
            return ExecuteTransactional(_queryDbSet, c => c.Max(selector), readUncommitted);
        }

        /// <summary>
        /// Get maximum value of selector item with predication
        /// </summary>
        /// <typeparam name="TField">Field to get maximum value</typeparam>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="selector">Field selector</param>
        /// <returns>Maximum value</returns>
        public TField Max<TField>(Expression<Func<TEntity, TField>> selector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true)
        {
            var query = _queryDbSet.Where(predicate);
            return ExecuteTransactional(query, c => c.Max(selector), readUncommitted);
        }

        /// <summary>
        /// Get maximum value of selector item with FilterSet
        /// </summary>
        /// <typeparam name="TField">Field to get maximum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <param name="filterSet">FilterSet for filtering resource</param>
        /// <returns>Maximum value</returns>
        public TField Max<TField>(Expression<Func<TEntity, TField>> selector, EveFilterSet filterSet, bool readUncommitted = true)
        {
            //Adding Conditions
            var query = filterSet.ToSqlServerQueryable(_queryDbSet);

            return ExecuteTransactional(query, c => c.Max(selector), readUncommitted);
        }
        #endregion

        #region Min

        /// <summary>
        /// Get minimum value of selector item
        /// </summary>
        /// <typeparam name="TField">Field to get minimum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <returns>Minimum value</returns>
        public TField Min<TField>(Expression<Func<TEntity, TField>> selector, bool readUncommitted = true)
        {
            return ExecuteTransactional(_queryDbSet, c => c.Min(selector), readUncommitted);
        }

        /// <summary>
        /// Get minimum value of selector item with predication
        /// </summary>
        /// <typeparam name="TField">Field to get minimum value</typeparam>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="selector">Field selector</param>
        /// <returns>Minimum value</returns>
        public TField Min<TField>(Expression<Func<TEntity, TField>> selector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true)
        {
            var query = _queryDbSet.Where(predicate);
            return ExecuteTransactional(query, c => c.Min(selector), readUncommitted);
        }

        /// <summary>
        /// Get minimum value of selector item with FilterSet
        /// </summary>
        /// <typeparam name="TField">Field to get minimum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <param name="filterSet">FilterSet for filtering resource</param>
        /// <returns>Minimum value</returns>
        public TField Min<TField>(Expression<Func<TEntity, TField>> selector, EveFilterSet filterSet, bool readUncommitted = true)
        {
            //Adding Conditions
            var query = filterSet.ToSqlServerQueryable(_queryDbSet);
            return ExecuteTransactional(query, c => c.Min(selector), readUncommitted);
        }

        #endregion

        #region Sum
        /// <summary>
        /// Computes the sum of the sequence of int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public int Sum(Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();
            if (predicate != null)
                q = q.Where(predicate);

            return ExecuteTransactional(q, c => c.Sum(selector), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public int? Sum(Expression<Func<TEntity, int?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();
            if (predicate != null)
                q = q.Where(predicate);
            return ExecuteTransactional(q, c => c.Sum(selector), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public long Sum(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();
            if (predicate != null)
                q = q.Where(predicate);

            return ExecuteTransactional(q, c => c.Sum(selector), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public long? Sum(Expression<Func<TEntity, long?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();
            if (predicate != null)
                q = q.Where(predicate);

            return ExecuteTransactional(q, c => c.Sum(selector), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public decimal Sum(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();
            if (predicate != null)
                q = q.Where(predicate);

            return ExecuteTransactional(q, c => c.Sum(selector), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public decimal? Sum(Expression<Func<TEntity, decimal?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();
            if (predicate != null)
                q = q.Where(predicate);
            return ExecuteTransactional(q, c => c.Sum(selector), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public float Sum(Expression<Func<TEntity, float>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();
            if (predicate != null)
                q = q.Where(predicate);

            return ExecuteTransactional(q, c => c.Sum(selector), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public float? Sum(Expression<Func<TEntity, float?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();
            if (predicate != null)
                q = q.Where(predicate);

            return ExecuteTransactional(q, c => c.Sum(selector), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public double Sum(Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();
            if (predicate != null)
                q = q.Where(predicate);

            return ExecuteTransactional(q, c => c.Sum(selector), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public double? Sum(Expression<Func<TEntity, double?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();
            if (predicate != null)
                q = q.Where(predicate);

            return ExecuteTransactional(q, c => c.Sum(selector), readUncommitted);
        }
        #endregion

        #region GroupedSum
        //public GroupedAggregationOutput<TKey, long?, TPaging> GroupedSum<TKey, TPaging>(Expression<Func<TEntity, TKey>> keySelector, Func<TEntity, long?> sumKeySelector, TPaging paging, Expression<Func<TEntity, bool>> predicate = null) where TPaging : OldBasePagination, new()
        //{
        //    return ExecuteFaultHandledOperation(() =>
        //    {
        //        var query = _queryDbSet.AsQueryable();
        //        if (predicate != null)
        //            query = query.Where(predicate);

        //        var groupedQuery = query.GroupBy(keySelector)
        //                .Select(group =>
        //                            new AggregationResult<TKey, long?>(AggregationFunction.Sum)
        //                            {
        //                                Group = group.Key,
        //                                Aggregation = group.Sum(sumKeySelector)
        //                            });

        //        if (paging.Type == typeof(PageNumberPagination))
        //        {
        //            ((PageNumberPagination)(OldBasePagination)paging).TotalCount = groupedQuery.Count();
        //        }

        //        if (paging != null)
        //            groupedQuery = groupedQuery.AddPaging(paging);

        //        var result = new GroupedAggregationOutput<TKey, long?, TPaging>()
        //        {
        //            Result = groupedQuery.ToList(),
        //            Paging = paging
        //        };


        //        return result;
        //    });
        //}

        /// <summary>
        /// Computes the sum of the sequence of int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Dictionary<TKey, int> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, int> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);

            var result = query;
            return ExecuteTransactional(result, c => c.GroupBy(keySelector).Select(group =>
                                  new
                                  {
                                      group.Key,
                                      Sum = group.Sum(sumKeySelector)
                                  }).ToDictionary(res => res.Key, res => res.Sum), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Dictionary<TKey, int?> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, int?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);

            return ExecuteTransactional(query, c => c.GroupBy(keySelector)
                     .Select(group =>
                                 new
                                 {
                                     group.Key,
                                     Sum = group.Sum(sumKeySelector)
                                 })
                     .ToDictionary(res => res.Key, res => res.Sum), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Dictionary<TKey, long> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, long> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);

            return ExecuteTransactional(query, c => c.GroupBy(keySelector)
                     .Select(group =>
                                 new
                                 {
                                     group.Key,
                                     Sum = group.Sum(sumKeySelector)
                                 })
                     .ToDictionary(res => res.Key, res => res.Sum), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Dictionary<TKey, long?> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, long?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);

            return ExecuteTransactional(query, c => c.GroupBy(keySelector)
                     .Select(group =>
                                 new
                                 {
                                     group.Key,
                                     Sum = group.Sum(sumKeySelector)
                                 })
                     .ToDictionary(res => res.Key, res => res.Sum), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Dictionary<TKey, decimal> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, decimal> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);

            return ExecuteTransactional(query, c => c.GroupBy(keySelector)
                     .Select(group =>
                                 new
                                 {
                                     group.Key,
                                     Sum = group.Sum(sumKeySelector)
                                 })
                     .ToDictionary(res => res.Key, res => res.Sum), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Dictionary<TKey, decimal?> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, decimal?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);
            return ExecuteTransactional(query, c => c.GroupBy(keySelector)
                     .Select(group =>
                                 new
                                 {
                                     group.Key,
                                     Sum = group.Sum(sumKeySelector)
                                 })
                     .ToDictionary(res => res.Key, res => res.Sum), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Dictionary<TKey, float> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, float> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);

            return ExecuteTransactional(query, c => c.GroupBy(keySelector)
                     .Select(group =>
                                 new
                                 {
                                     group.Key,
                                     Sum = group.Sum(sumKeySelector)
                                 })
                     .ToDictionary(res => res.Key, res => res.Sum), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Dictionary<TKey, float?> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, float?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);

            return ExecuteTransactional(query, c => c.GroupBy(keySelector)
                     .Select(group =>
                                 new
                                 {
                                     group.Key,
                                     Sum = group.Sum(sumKeySelector)
                                 })
                     .ToDictionary(res => res.Key, res => res.Sum), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Dictionary<TKey, double> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, double> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);

            return ExecuteTransactional(query, c => c.GroupBy(keySelector)
                     .Select(group =>
                                 new
                                 {
                                     group.Key,
                                     Sum = group.Sum(sumKeySelector)
                                 })
                     .ToDictionary(res => res.Key, res => res.Sum), readUncommitted);
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Dictionary<TKey, double?> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, double?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var query = _queryDbSet.AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);

            return ExecuteTransactional(query, c => c.GroupBy(keySelector)
                     .Select(group =>
                                 new
                                 {
                                     group.Key,
                                     Sum = group.Sum(sumKeySelector)
                                 })
                     .ToDictionary(res => res.Key, res => res.Sum), readUncommitted);
        }
        #endregion

        #region Count
        public long Count(bool readUncommitted = true)
        {
            return ExecuteTransactional(_queryDbSet, c => c.LongCount(), readUncommitted);
        }
        public long Count(Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true)
        {
            return ExecuteTransactional(_queryDbSet, c => c.Where(predicate).Count(), readUncommitted);
        }


        public long Count<TResult>(Expression<Func<TEntity, TResult>> distinctSelector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true)
        {
            var query = _queryDbSet
                .Where(predicate)
                .Select(distinctSelector)
                .Distinct();
            return ExecuteTransactional(query, c => c.Count(), readUncommitted);
        }

        public long Count(EveFilterSet filterset, bool readUncommitted = true)
        {
            var query = filterset.ToSqlServerQueryable(_queryDbSet);
            return ExecuteTransactional(query, c => c.LongCount(), readUncommitted);
        }
        #endregion

        #region GroupedCount
        /// <summary>
        /// Group count
        /// </summary>
        /// <typeparam name="TKey">Tkey</typeparam>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <returns></returns>
        public Dictionary<TKey, int> GroupedCount<TKey>(Func<TEntity, TKey> keySelector, bool readUncommitted = true)
        {
            return ExecuteTransactional(_queryDbSet, c => c.GroupBy(keySelector)
                               .Select(group =>
                                    new
                                    {
                                        group.Key,
                                        Count = group.Count()
                                    }).ToDictionary(res => res.Key, res => res.Count), readUncommitted);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Dictionary<TKey, int> GroupedCount<TKey>(Func<TEntity, TKey> keySelector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true)
        {
            return ExecuteTransactional(_queryDbSet, c => c.Where(predicate)
                                .GroupBy(keySelector)
                                .Select(group =>
                                     new
                                     {
                                         group.Key,
                                         Count = group.Count()
                                     }).ToDictionary(res => res.Key, res => res.Count), readUncommitted);
        }
        #endregion

        public TEntity GetById(long id, List<string> joins = null, bool readUncommitted = true)
        {
            return FirstOrDefault($"Id = {id}", joins: joins);
        }
        public IQueryable<TEntity> GetDbSet(bool readUncommitted = true)
        {
            return _queryDbSet;
        }

        #endregion

        #region Async Method


        #region GetAll
        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <summary xml:lang="fa">
        /// را بر می گرداند TEntity تمام موجودیت ها از نوع
        /// </summary>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        public Task<IEnumerable<TEntity>> GetAllAsync(int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GetAll(maxCount, orderByDictionary, joins, readUncommitted));
            return taskResult;
        }
        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <typeparam name="TResult">Type of return value items</typeparam>
        /// <param name="selector">A transform function to apply to each elemnt</param>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        public Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GetAll(selector, maxCount, orderByDictionary, joins, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <param name="paging">Indicate result paging information</param>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        public Task<EveOutputParam<TEntity, EveBasePagination>> GetAllAsync(EveBasePagination paging, int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GetAll(paging, maxCount, orderByDictionary, joins, readUncommitted));
            return taskResult;
        }
        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <typeparam name="TResult">Type of return value items</typeparam>
        /// <param name="paging">Indicate result paging information</param>
        /// <param name="selector">A transform function to apply to each elemnt</param>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        public Task<EveOutputParam<TResult, EveBasePagination>> GetAllAsync<TResult>(EveBasePagination paging, Expression<Func<TEntity, TResult>> selector, int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GetAll(paging, selector, maxCount, orderByDictionary, joins, readUncommitted));
            return taskResult;
        }
        #endregion

        #region Distinct
        /// <summary>
        /// Returns distinct elements from the <paramref name="TEntity"/> collection by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector">A transform function to apply to each element and compare for their equality</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="comparer">An IEqualityComparare<in <paramref name="TResult"/>> to campare  values.</param>
        /// <returns></returns>
        public Task<List<TResult>> DistinctAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, IEqualityComparer<TResult> comparer = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Distinct(selector, predicate, comparer, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Returns distinct elements from the <paramref name="TEntity"/> collection by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TPaging"></typeparam>
        /// <param name="selector">A transform function to apply to each element and compare for their equality</param>
        /// <param name="input"></param>
        /// <param name="comparer">An IEqualityComparare<in <paramref name="TResult"/>> to campare  values.</param>
        /// <returns></returns>
        public Task<EveOutputParam<TResult, TPaging>> DistinctAsync<TResult, TPaging>(Expression<Func<TEntity, TResult>> selector, EveInputParam<TEntity, TPaging> input, IEqualityComparer<TResult> comparer = null, bool readUncommitted = true) where TPaging : EveBasePagination, new()
        {
            var taskResult = Task.Run(() => Distinct(selector, input, comparer, readUncommitted));
            return taskResult;
        }
        #endregion

        #region First
        /// <summary>
        /// Returns the first element of the <paramref name="TEntity"/> collectio that satisfies a specefic condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns></returns>
        public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => First(predicate, orderByDictionary, joins, readUncommitted));
            return taskResult;
        }

        public Task<TResult> FirstAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => First(selector, predicate, orderByDictionary, readUncommitted));
            return taskResult;
        }
        #endregion

        #region FirstOrDefault
        /// <summary>
        /// Returns the first element of the <paramref name="TEntity"/> collectio that satisfies a specefic condition or a default value if no such element is found.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns></returns>
        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => FirstOrDefault(predicate, orderByDictionary, joins, readUncommitted));
            return taskResult;
        }

        public Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => FirstOrDefault(selector, predicate, orderByDictionary, readUncommitted));
            return taskResult;
        }
        #endregion

        #region Find
        /// <summary>
        /// Search with lambda expression
        /// </summary>
        /// <typeparam name="Expression<Func<TEntity, bool>>">Type of Id</typeparam>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of joined entities</param>
        public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Find(predicate, orderByDictionary, joins, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Search with lambda expression
        /// </summary>
        /// <typeparam name="Expression<Func<TEntity, bool>>">Type of Id</typeparam>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="selector">A transform function to apply to each element</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of joined entities</param>
        public Task<List<TResult>> FindAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Find(predicate, selector, orderByDictionary, joins));
            return taskResult;
        }

        /// <summary>
        /// Search with lambda expression
        /// </summary>
        /// <typeparam name="Expression<Func<TEntity, bool>>">Type of Id</typeparam>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="selector">A transform function to apply to each element</param>
        /// <param name="pagination">An object to determine pagination of the result</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of joined entities</param>
        public Task<EveOutputParam<TResult, TPaging>> FindAsync<TResult, TPaging>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, TPaging pagination, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true) where TPaging : EveBasePagination, new()
        {
            var taskResult = Task.Run(() => Find(predicate, selector, pagination, orderByDictionary, joins, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Finds objects with type of <see cref="TEntity"></see> whit input type of <see cref="EveInputParam{TEntity, TPaging}"/>
        /// </summary>
        /// <typeparam name="TPaging">Type of Paging</typeparam>
        /// <param name="input"></param>
        /// <returns><see cref="EveOutputParam{TEntity, TPaging}"/></returns>
        public Task<EveOutputParam<TEntity, TPaging>> FindAsync<TPaging>(EveInputParam<TEntity, TPaging> input, bool readUncommitted = true) where TPaging : EveBasePagination, new()
        {
            var taskResult = Task.Run(() => Find(input, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Finds objects with type of <see cref="TEntity"></see> whit input type of <see cref="EveInputParam{TEntity, TPaging}"/>
        /// </summary>
        /// <typeparam name="TPaging">Type of Paging</typeparam>
        /// <param name="input"></param>
        /// <returns><see cref="EveOutputParam{TEntity, TPaging}"/></returns>
        public Task<EveOutputParam<TResult, TPaging>> FindAsync<TResult, TPaging>(EveInputParam<TEntity, TPaging> input, Expression<Func<TEntity, TResult>> selector, bool readUncommitted = true) where TPaging : EveBasePagination, new()
        {
            var taskResult = Task.Run(() => Find(input, selector, readUncommitted));
            return taskResult;
        }

        #endregion

        #region Any
        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Any(predicate, readUncommitted));
            return taskResult;
        }
        #endregion

        #region Max

        /// <summary>
        /// Get maximum value of selector item
        /// </summary>
        /// <typeparam name="TField">Field to get maximum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <returns>Maximum value</returns>
        public Task<TField> MaxAsync<TField>(Expression<Func<TEntity, TField>> selector, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Max(selector, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Get maximum value of selector item with predication
        /// </summary>
        /// <typeparam name="TField">Field to get maximum value</typeparam>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="selector">Field selector</param>
        /// <returns>Maximum value</returns>
        public Task<TField> MaxAsync<TField>(Expression<Func<TEntity, TField>> selector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Max(selector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Get maximum value of selector item with FilterSet
        /// </summary>
        /// <typeparam name="TField">Field to get maximum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <param name="filterSet">FilterSet for filtering resource</param>
        /// <returns>Maximum value</returns>
        public Task<TField> MaxAsync<TField>(Expression<Func<TEntity, TField>> selector, EveFilterSet filterSet, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Max(selector, filterSet, readUncommitted));
            return taskResult;
        }
        #endregion

        #region Min

        /// <summary>
        /// Get minimum value of selector item
        /// </summary>
        /// <typeparam name="TField">Field to get minimum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <returns>Minimum value</returns>
        public Task<TField> MinAsync<TField>(Expression<Func<TEntity, TField>> selector, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Min(selector, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Get minimum value of selector item with predication
        /// </summary>
        /// <typeparam name="TField">Field to get minimum value</typeparam>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="selector">Field selector</param>
        /// <returns>Minimum value</returns>
        public Task<TField> MinAsync<TField>(Expression<Func<TEntity, TField>> selector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Min(selector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Get minimum value of selector item with FilterSet
        /// </summary>
        /// <typeparam name="TField">Field to get minimum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <param name="filterSet">FilterSet for filtering resource</param>
        /// <returns>Minimum value</returns>
        public Task<TField> MinAsync<TField>(Expression<Func<TEntity, TField>> selector, EveFilterSet filterSet, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Min(selector, filterSet, readUncommitted));
            return taskResult;
        }

        #endregion

        #region Sum
        /// <summary>
        /// Computes the sum of the sequence of int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<int> SumAsync(Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Sum(selector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<int?> SumAsync(Expression<Func<TEntity, int?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Sum(selector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<long> SumAsync(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Sum(selector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<long?> SumAsync(Expression<Func<TEntity, long?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Sum(selector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Sum(selector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<decimal?> SumAsync(Expression<Func<TEntity, decimal?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Sum(selector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<float> SumAsync(Expression<Func<TEntity, float>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Sum(selector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<float?> SumAsync(Expression<Func<TEntity, float?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Sum(selector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<double> SumAsync(Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Sum(selector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<double?> SumAsync(Expression<Func<TEntity, double?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Sum(selector, predicate, readUncommitted));
            return taskResult;
        }
        #endregion

        #region GroupedSum
        /// <summary>
        /// Computes the sum of the sequence of int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<Dictionary<TKey, int>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, int> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GroupedSum(keySelector, sumKeySelector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<Dictionary<TKey, int?>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, int?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GroupedSum(keySelector, sumKeySelector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<Dictionary<TKey, long>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, long> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GroupedSum(keySelector, sumKeySelector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<Dictionary<TKey, long?>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, long?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GroupedSum(keySelector, sumKeySelector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<Dictionary<TKey, decimal>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, decimal> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GroupedSum(keySelector, sumKeySelector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<Dictionary<TKey, decimal?>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, decimal?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GroupedSum(keySelector, sumKeySelector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<Dictionary<TKey, float>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, float> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GroupedSum(keySelector, sumKeySelector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<Dictionary<TKey, float?>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, float?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GroupedSum(keySelector, sumKeySelector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<Dictionary<TKey, double>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, double> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GroupedSum(keySelector, sumKeySelector, predicate, readUncommitted));
            return taskResult;
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns></returns>
        public Task<Dictionary<TKey, double?>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, double?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GroupedSum(keySelector, sumKeySelector, predicate, readUncommitted));
            return taskResult;
        }
        #endregion

        #region Count
        public Task<long> CountAsync(bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Count(readUncommitted));
            return taskResult;
        }
        public Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Count(predicate, readUncommitted));
            return taskResult;
        }

        public Task<long> CountAsync<TResult>(Expression<Func<TEntity, TResult>> distinctSelector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Count(distinctSelector, predicate, readUncommitted));
            return taskResult;
        }
        public Task<long> CountAsync(EveFilterSet filterset, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => Count(filterset, readUncommitted));
            return taskResult;
        }
        #endregion

        #region GroupedCount
        public Task<Dictionary<TKey, int>> GroupedCountAsync<TKey>(Func<TEntity, TKey> keySelector, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GroupedCount(keySelector, readUncommitted));
            return taskResult;
        }
        public Task<Dictionary<TKey, int>> GroupedCountAsync<TKey>(Func<TEntity, TKey> keySelector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GroupedCount(keySelector, predicate, readUncommitted));
            return taskResult;
        }
        #endregion

        public Task<TEntity> GetByIdAsync(long id, List<string> joins = null, bool readUncommitted = true)
        {
            var taskResult = Task.Run(() => GetById(id, joins, readUncommitted));
            return taskResult;
        }

        #endregion
        protected TResult ExecuteTransactional<TInput, TResult>(IQueryable<TInput> query, Func<IQueryable<TInput>, TResult> queryAction, bool readUncommitted)
        {


            //If there is already an open transaction
            if (_dbContext.Database.CurrentTransaction != null)
                return queryAction(query);

            var isolationLevel = readUncommitted ? IsolationLevel.ReadUncommitted : IsolationLevel.ReadCommitted;
            using (var transaction = _dbContext.Database.BeginTransaction(isolationLevel))
            {
                //The explicit call to dbContextTransaction.Rollback() is  unnecessary, because disposing the transaction at the end of the using block will take care of rolling back.
                //If an error of sufficient severity occurs in SQL Server, the transaction will get automatically rolled back, and the call to dbContextTransaction.Rollback() in the catch block will actually fail.
                var result = queryAction(query);
                transaction.Commit();
                return result;
            }
        }

        /// <summary>
        /// Returns the first element of the <paramref name="TEntity"/> collectio that satisfies a specefic condition or a default value if no such element is found.
        /// </summary>
        /// <param name="predicate">A string to check each element for a condition.</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns></returns>
        private TEntity FirstOrDefault(string predicate, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true)
        {
            var q = _queryDbSet.AsQueryable();

            if (joins?.Count() > 0)
                q = q.ToJoined(joins);

            if (predicate != null)
                q = q.Where(predicate);

            //Adding Ordering
            if (orderByDictionary?.Count() > 0)
                q = q.OrderBy(SortGenerator.GetSortString(orderByDictionary));

            return ExecuteTransactional(q, c => c.FirstOrDefault(), readUncommitted);
        }
    }
}
