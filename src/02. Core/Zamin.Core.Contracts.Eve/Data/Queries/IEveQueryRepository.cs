using System.Linq.Expressions;
using static Zamin.Core.Contracts.Eve.Data.Queries.EveQueryEnums;

namespace Zamin.Core.Contracts.Eve.Data.Queries
{
    public interface IEveQueryRepository<TEntity> where TEntity : class
    {
        #region Normal


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
        IEnumerable<TEntity> GetAll(int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);

        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <typeparam name="TResult">Type of return value items</typeparam>
        /// <param name="selector">A transform function to apply to each element</param>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector, int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);

        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <param name="paging">Indicate result paging information</param>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        EveOutputParam<TEntity, EveBasePagination> GetAll(EveBasePagination paging, int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);

        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <typeparam name="TResult">Type of return value items</typeparam>
        /// <param name="paging">Indicate result paging information</param>
        /// <param name="selector">A transform function to apply to each element</param>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        EveOutputParam<TResult, EveBasePagination> GetAll<TResult>(EveBasePagination paging, Expression<Func<TEntity, TResult>> selector, int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);
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
        List<TResult> Distinct<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, IEqualityComparer<TResult> comparer = null, bool readUncommitted = true);

        /// <summary>
        /// Returns distinct elements from the <paramref name="TEntity"/> collection by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TPaging"></typeparam>
        /// <param name="selector">A transform function to apply to each element and compare for their equality</param>
        /// <param name="input"></param>
        /// <param name="comparer">An IEqualityComparare<in <paramref name="TResult"/>> to campare  values.</param>
        /// <returns></returns>
        EveOutputParam<TResult, TPaging> Distinct<TResult, TPaging>(Expression<Func<TEntity, TResult>> selector, EveInputParam<TEntity, TPaging> input, IEqualityComparer<TResult> comparer = null, bool readUncommitted = true) where TPaging : EveBasePagination, new();
        #endregion

        #region First
        /// <summary>
        /// Returns the first element of the <paramref name="TEntity"/> collectio that satisfies a specefic condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns></returns>
        TEntity First(Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);

        TResult First<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, bool readUncommitted = true);
        #endregion

        #region FirstOrDefault
        /// <summary>
        /// Returns the first element of the <paramref name="TEntity"/> collectio that satisfies a specefic condition or a default value if no such element is found.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns></returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);

        TResult FirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, bool readUncommitted = true);
        #endregion

        #region Find
        /// <summary>
        /// Search with lambda expression
        /// </summary>
        /// <typeparam name="Expression<Func<TEntity, bool>>">Type of Id</typeparam>
        /// <param name="Predicate"></param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of joined entities</param>
        List<TEntity> Find(Expression<Func<TEntity, bool>> predicate, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);

        /// <summary>
        /// Search with lambda expression
        /// </summary>
        /// <typeparam name="Expression<Func<TEntity, bool>>">Type of Id</typeparam>
        /// <param name="Predicate"></param>
        /// <param name="selector">A transform function to apply to each element</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of joined entities</param>
        List<TResult> Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);

        /// <summary>
        /// Search with lambda expression
        /// </summary>
        /// <typeparam name="Expression<Func<TEntity, bool>>">Type of Id</typeparam>
        /// <param name="Predicate"></param>
        /// <param name="selector">A transform function to apply to each element</param>
        /// <param name="pagination">An object to determine pagination of the result</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of joined entities</param>
        EveOutputParam<TResult, TPaging> Find<TResult, TPaging>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, TPaging pagination, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true) where TPaging : EveBasePagination, new();

        /// <summary>
        /// Finds objects with type of <see cref="TEntity"></see> whit input type of <see cref="EveInputParam{TEntity, TPaging}"/>
        /// </summary>
        /// <typeparam name="TPaging">Type of Paging</typeparam>
        /// <param name="input"></param>
        /// <returns><see cref="OutputParam{TEntity, TPaging}"/></returns>
        EveOutputParam<TEntity, TPaging> Find<TPaging>(EveInputParam<TEntity, TPaging> input, bool readUncommitted = true) where TPaging : EveBasePagination, new();

        /// <summary>
        /// Finds objects with type of <see cref="TEntity"></see> whit input type of <see cref="EveInputParam{TEntity, TPaging}"/>
        /// </summary>
        /// <typeparam name="TPaging">Type of Paging</typeparam>
        /// <param name="input"></param>
        /// <returns><see cref="OutputParam{TEntity, TPaging}"/></returns>
        EveOutputParam<TResult, TPaging> Find<TResult, TPaging>(EveInputParam<TEntity, TPaging> input, Expression<Func<TEntity, TResult>> selector, bool readUncommitted = true) where TPaging : EveBasePagination, new();

        #endregion

        #region Any
        bool Any(Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);
        #endregion

        #region Max

        /// <summary>
        /// Get maximum value of selector item
        /// </summary>
        /// <typeparam name="TField">Field to get maximum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <returns>Maximum value</returns>
        TField Max<TField>(Expression<Func<TEntity, TField>> selector, bool readUncommitted = true);

        /// <summary>
        /// Get maximum value of selector item with predication
        /// </summary>
        /// <typeparam name="TField">Field to get maximum value</typeparam>
        /// <param name="predicate">Predication over resource</param>
        /// <param name="selector">Field selector</param>
        /// <returns>Maximum value</returns>
        TField Max<TField>(Expression<Func<TEntity, TField>> selector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true);

        /// <summary>
        /// Get maximum value of selector item with FilterSet
        /// </summary>
        /// <typeparam name="TField">Field to get maximum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <param name="filterSet">FilterSet for filtering resource</param>
        /// <returns>Maximum value</returns>
        TField Max<TField>(Expression<Func<TEntity, TField>> selector, EveFilterSet filterSet, bool readUncommitted = true);

        #endregion

        #region Min
        /// <summary>
        /// Get minimum value of selector item
        /// </summary>
        /// <typeparam name="TField">Field to get minimum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <returns>Minimum value</returns>
        TField Min<TField>(Expression<Func<TEntity, TField>> selector, bool readUncommitted = true);

        /// <summary>
        /// Get minimum value of selector item with predication
        /// </summary>
        /// <typeparam name="TField">Field to get minimum value</typeparam>
        /// <param name="predicate">Predication over resource</param>
        /// <param name="selector">Field selector</param>
        /// <returns>Minimum value</returns>
        TField Min<TField>(Expression<Func<TEntity, TField>> selector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true);

        /// <summary>
        /// Get minimum value of selector item with FilterSet
        /// </summary>
        /// <typeparam name="TField">Field to get minimum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <param name="filterSet">FilterSet for filtering resource</param>
        /// <returns>Minimum value</returns>
        TField Min<TField>(Expression<Func<TEntity, TField>> selector, EveFilterSet filterSet, bool readUncommitted = true);
        #endregion

        #region Sum
        /// <summary>
        /// Computes the sum of the sequence of int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        int Sum(Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        int? Sum(Expression<Func<TEntity, int?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        long Sum(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        long? Sum(Expression<Func<TEntity, long?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        decimal Sum(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        decimal? Sum(Expression<Func<TEntity, decimal?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        float Sum(Expression<Func<TEntity, float>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        float? Sum(Expression<Func<TEntity, float?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        double Sum(Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        double? Sum(Expression<Func<TEntity, double?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);
        #endregion

        #region GroupedSum
        /// <summary>
        /// Computes the sum of the sequence of int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Dictionary<TKey, int> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, int> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Dictionary<TKey, int?> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, int?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Dictionary<TKey, long> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, long> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Dictionary<TKey, long?> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, long?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Dictionary<TKey, decimal> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, decimal> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Dictionary<TKey, decimal?> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, decimal?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Dictionary<TKey, float> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, float> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Dictionary<TKey, float?> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, float?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Dictionary<TKey, double> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, double> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Dictionary<TKey, double?> GroupedSum<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, double?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);
        #endregion

        #region Count
        long Count(bool readUncommitted = true);
        long Count(Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true);
        long Count<TResult>(Expression<Func<TEntity, TResult>> distinctSelector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true);
        long Count(EveFilterSet filterset, bool readUncommitted = true);
        #endregion

        #region GroupedCount
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <returns></returns>
        Dictionary<TKey, int> GroupedCount<TKey>(Func<TEntity, TKey> keySelector, bool readUncommitted = true);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Dictionary<TKey, int> GroupedCount<TKey>(Func<TEntity, TKey> keySelector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true);
        #endregion

        /// <summary>
        /// Get qurable expression which ready to query
        /// </summary>
        IQueryable<TEntity> GetDbSet(bool readUncommitted = true);

        /// <summary>
        ///  Get The object of type <see cref="TEntity"/> By its Id
        /// </summary>
        /// <typeparam name="string">Type of Id</typeparam>
        /// <param name="id"></param>
        /// <returns>The Object of Type <see cref="TEntity"/></returns>
        TEntity GetById(long id, List<string> joins = null, bool readUncommitted = true);


        #endregion

        #region Async


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
        Task<IEnumerable<TEntity>> GetAllAsync(int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);
        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <typeparam name="TResult">Type of return value items</typeparam>
        /// <param name="selector">A transform function to apply to each element</param>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);

        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <param name="paging">Indicate result paging information</param>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        Task<EveOutputParam<TEntity, EveBasePagination>> GetAllAsync(EveBasePagination paging, int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);
        /// <summary>
        /// Gets All objects of type <see cref="TEntity"/> based on type of Paging
        /// </summary>
        /// <typeparam name="TResult">Type of return value items</typeparam>
        /// <param name="paging">Indicate result paging information</param>
        /// <param name="selector">A transform function to apply to each element</param>
        /// <param name="maxCount">Max return items count</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns>All Objects of type <see cref="IEnumerable{TEntity}"/></returns>
        Task<EveOutputParam<TResult, EveBasePagination>> GetAllAsync<TResult>(EveBasePagination paging, Expression<Func<TEntity, TResult>> selector, int maxCount = 500, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);
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
        Task<List<TResult>> DistinctAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, IEqualityComparer<TResult> comparer = null, bool readUncommitted = true);

        /// <summary>
        /// Returns distinct elements from the <paramref name="TEntity"/> collection by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TPaging"></typeparam>
        /// <param name="selector">A transform function to apply to each element and compare for their equality</param>
        /// <param name="input"></param>
        /// <param name="comparer">An IEqualityComparare<in <paramref name="TResult"/>> to campare  values.</param>
        /// <returns></returns>
        Task<EveOutputParam<TResult, TPaging>> DistinctAsync<TResult, TPaging>(Expression<Func<TEntity, TResult>> selector, EveInputParam<TEntity, TPaging> input, IEqualityComparer<TResult> comparer = null, bool readUncommitted = true) where TPaging : EveBasePagination, new();
        #endregion

        #region First
        /// <summary>
        /// Returns the first element of the <paramref name="TEntity"/> collectio that satisfies a specefic condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns></returns>
        Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);

        Task<TResult> FirstAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, bool readUncommitted = true);
        #endregion

        #region FirstOrDefault
        /// <summary>
        /// Returns the first element of the <paramref name="TEntity"/> collectio that satisfies a specefic condition or a default value if no such element is found.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="joins">List of entity attributes to be joined</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);

        Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate = null, Dictionary<string, SortDirection> orderByDictionary = null, bool readUncommitted = true);
        #endregion

        #region Find
        /// <summary>
        /// Search with lambda expression
        /// </summary>
        /// <typeparam name="Expression<Func<TEntity, bool>>">Type of Id</typeparam>
        /// <param name="Predicate"></param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of joined entities</param>
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);

        /// <summary>
        /// Search with lambda expression
        /// </summary>
        /// <typeparam name="Expression<Func<TEntity, bool>>">Type of Id</typeparam>
        /// <param name="Predicate"></param>
        /// <param name="selector">A transform function to apply to each element</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of joined entities</param>
        Task<List<TResult>> FindAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true);

        /// <summary>
        /// Search with lambda expression
        /// </summary>
        /// <typeparam name="Expression<Func<TEntity, bool>>">Type of Id</typeparam>
        /// <param name="Predicate"></param>
        /// <param name="selector">A transform function to apply to each element</param>
        /// <param name="pagination">An object to determine pagination of the result</param>
        /// <param name="orderByDictionary">A dictionary to represent result ordering</param>
        /// <param name="joins">List of joined entities</param>
        Task<EveOutputParam<TResult, TPaging>> FindAsync<TResult, TPaging>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, TPaging pagination, Dictionary<string, SortDirection> orderByDictionary = null, List<string> joins = null, bool readUncommitted = true) where TPaging : EveBasePagination, new();

        /// <summary>
        /// Finds objects with type of <see cref="TEntity"></see> whit input type of <see cref="EveInputParam{TEntity, TPaging}"/>
        /// </summary>
        /// <typeparam name="TPaging">Type of Paging</typeparam>
        /// <param name="input"></param>
        /// <returns><see cref="OutputParam{TEntity, TPaging}"/></returns>
        Task<EveOutputParam<TEntity, TPaging>> FindAsync<TPaging>(EveInputParam<TEntity, TPaging> input, bool readUncommitted = true) where TPaging : EveBasePagination, new();

        /// <summary>
        /// Finds objects with type of <see cref="TEntity"></see> whit input type of <see cref="EveInputParam{TEntity, TPaging}"/>
        /// </summary>
        /// <typeparam name="TPaging">Type of Paging</typeparam>
        /// <param name="input"></param>
        /// <returns><see cref="OutputParam{TEntity, TPaging}"/></returns>
        Task<EveOutputParam<TResult, TPaging>> FindAsync<TResult, TPaging>(EveInputParam<TEntity, TPaging> input, Expression<Func<TEntity, TResult>> selector, bool readUncommitted = true) where TPaging : EveBasePagination, new();

        #endregion

        #region Any
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);
        #endregion

        #region Max

        /// <summary>
        /// Get maximum value of selector item
        /// </summary>
        /// <typeparam name="TField">Field to get maximum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <returns>Maximum value</returns>
        Task<TField> MaxAsync<TField>(Expression<Func<TEntity, TField>> selector, bool readUncommitted = true);

        /// <summary>
        /// Get maximum value of selector item with predication
        /// </summary>
        /// <typeparam name="TField">Field to get maximum value</typeparam>
        /// <param name="predicate">Predication over resource</param>
        /// <param name="selector">Field selector</param>
        /// <returns>Maximum value</returns>
        Task<TField> MaxAsync<TField>(Expression<Func<TEntity, TField>> selector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true);

        /// <summary>
        /// Get maximum value of selector item with FilterSet
        /// </summary>
        /// <typeparam name="TField">Field to get maximum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <param name="filterSet">FilterSet for filtering resource</param>
        /// <returns>Maximum value</returns>
        Task<TField> MaxAsync<TField>(Expression<Func<TEntity, TField>> selector, EveFilterSet filterSet, bool readUncommitted = true);

        #endregion

        #region Min
        /// <summary>
        /// Get minimum value of selector item
        /// </summary>
        /// <typeparam name="TField">Field to get minimum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <returns>Minimum value</returns>
        Task<TField> MinAsync<TField>(Expression<Func<TEntity, TField>> selector, bool readUncommitted = true);

        /// <summary>
        /// Get minimum value of selector item with predication
        /// </summary>
        /// <typeparam name="TField">Field to get minimum value</typeparam>
        /// <param name="predicate">Predication over resource</param>
        /// <param name="selector">Field selector</param>
        /// <returns>Minimum value</returns>
        Task<TField> MinAsync<TField>(Expression<Func<TEntity, TField>> selector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true);

        /// <summary>
        /// Get minimum value of selector item with FilterSet
        /// </summary>
        /// <typeparam name="TField">Field to get minimum value</typeparam>
        /// <param name="selector">Field selector</param>
        /// <param name="filterSet">FilterSet for filtering resource</param>
        /// <returns>Minimum value</returns>
        Task<TField> MinAsync<TField>(Expression<Func<TEntity, TField>> selector, EveFilterSet filterSet, bool readUncommitted = true);
        #endregion

        #region Sum
        /// <summary>
        /// Computes the sum of the sequence of int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<int> SumAsync(Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<int?> SumAsync(Expression<Func<TEntity, int?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<long> SumAsync(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<long?> SumAsync(Expression<Func<TEntity, long?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<decimal?> SumAsync(Expression<Func<TEntity, decimal?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<float> SumAsync(Expression<Func<TEntity, float>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<float?> SumAsync(Expression<Func<TEntity, float?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<double> SumAsync(Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<double?> SumAsync(Expression<Func<TEntity, double?>> selector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);
        #endregion

        #region GroupedSum
        /// <summary>
        /// Computes the sum of the sequence of int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<Dictionary<TKey, int>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, int> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable int values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<Dictionary<TKey, int?>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, int?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<Dictionary<TKey, long>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, long> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable long values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<Dictionary<TKey, long?>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, long?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<Dictionary<TKey, decimal>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, decimal> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable decimal values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<Dictionary<TKey, decimal?>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, decimal?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<Dictionary<TKey, float>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, float> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable float values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<Dictionary<TKey, float?>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, float?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<Dictionary<TKey, double>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, double> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);

        /// <summary>
        /// Computes the sum of the sequence of nullable double values that is obtained by invoking a projection function on the elements of the input sequence wich saticfy the given predicate.
        /// </summary>
        /// <param name="keySelector">A function to extract the key for each element</param>
        /// <param name="sumKeySelector">A projection function to select the key for sum in each element.</param>
        /// <param name="predicate">Predication over resource</param>
        /// <returns></returns>
        Task<Dictionary<TKey, double?>> GroupedSumAsync<TKey>(Func<TEntity, TKey> keySelector, Func<TEntity, double?> sumKeySelector, Expression<Func<TEntity, bool>> predicate = null, bool readUncommitted = true);
        #endregion

        #region Count
        Task<long> CountAsync(bool readUncommitted = true);
        Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true);
        Task<long> CountAsync<TResult>(Expression<Func<TEntity, TResult>> distinctSelector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true);
        Task<long> CountAsync(EveFilterSet filterset, bool readUncommitted = true);
        #endregion

        #region GroupedCount
        Task<Dictionary<TKey, int>> GroupedCountAsync<TKey>(Func<TEntity, TKey> keySelector, bool readUncommitted = true);
        Task<Dictionary<TKey, int>> GroupedCountAsync<TKey>(Func<TEntity, TKey> keySelector, Expression<Func<TEntity, bool>> predicate, bool readUncommitted = true);
        #endregion

        /// <summary>
        /// Get The object of type <see cref="TEntity"/> by its Id, Asynchronously
        /// </summary>
        /// <typeparam name="string">Type of Id</typeparam>
        /// <param name="id"></param>
        /// <returns>Awaitable <see cref= "TEntity"/></returns>
        Task<TEntity> GetByIdAsync(long id, List<string> joins = null, bool readUncommitted = true);

        #endregion
    }
}
