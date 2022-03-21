using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using Zamin.Core.Contracts.Eve.Data.Queries;
using Zamin.Utilities.Eve.Extensions;
using static Zamin.Core.Contracts.Eve.Data.Queries.EveQueryEnums;
using TypeExtensions = Zamin.Utilities.Eve.Extensions.TypeExtensions;

namespace Zamin.Infra.Data.Sql.Queries.Eve
{
    public static class Extensions
    {
        private static int _count = 0;

        /// <summary>
        /// Mapping for Dynamic Linq
        /// </summary>
        private static IDictionary<ConditionOperator, string> Operators = new Dictionary<ConditionOperator, string>
        {
            [ConditionOperator.Equal] = "=",
            [ConditionOperator.NotEqual] = "!=",
            [ConditionOperator.LessThan] = "<",
            [ConditionOperator.LessThanOrEqual] = "<=",
            [ConditionOperator.GreaterThan] = ">",
            [ConditionOperator.GreaterThanOrEqual] = ">=",
            [ConditionOperator.StartsWith] = "StartsWith",
            [ConditionOperator.EndsWith] = "EndsWith",
            [ConditionOperator.Like] = "Contains",
            [ConditionOperator.DoNotLike] = "Contains",
            [ConditionOperator.IsIn] = "Contains",
            [ConditionOperator.IsNotIn] = "Contains",
            [ConditionOperator.IsNull] = "=",
            [ConditionOperator.IsNotNull] = "!=",
        };

        /// <summary>
        /// Generate dynamic linq string 
        /// </summary>
        /// <typeparam name="T">type of source</typeparam>
        /// <param name="filter">filter model</param>
        /// <param name="start">specified for first time calling method (true by default)</param>
        /// <returns></returns>
        private static string ToDynamicLinqExpression<T>(this EveFilter filter, bool start = true)
        {
            if (start)
                _count = -1;
            _count++;

            var joinField = filter.Field.Split('.');

            bool isCollectionChild = false;
            string childName = "", collectionChildPropName = "";

            if (joinField.Length > 1)
            {
                childName = joinField[0].ToPascalCase();
                isCollectionChild = typeof(T).GetProperty(childName).PropertyType.GetGenericArguments().Any();

                if (isCollectionChild)
                    collectionChildPropName = joinField[1].ToPascalCase();
            }

            var expresion = "";
            string comparison = Operators[filter.Operator];


            switch (filter.Operator)
            {
                case ConditionOperator.StartsWith:
                case ConditionOperator.EndsWith:
                case ConditionOperator.Like:
                    {
                        if (isCollectionChild)
                            expresion = $"{childName}.Any({collectionChildPropName}.{comparison}(@{_count}))";
                        else
                            expresion = $"{filter.Field}.{comparison}(@{_count})";

                        break;
                    }
                case ConditionOperator.DoNotLike:
                    {
                        if (isCollectionChild)
                            expresion = $"{childName}.Any(!{collectionChildPropName}.{comparison}(@{_count}))";
                        else
                            expresion = $"!{filter.Field}.{comparison}(@{_count})";

                        break;
                    }
                case ConditionOperator.IsIn:
                    {
                        if (isCollectionChild)
                            expresion = $"{childName}.Any(@{_count}.{comparison}({collectionChildPropName}))";
                        else
                            expresion = $"@{_count}.{comparison}({filter.Field})";
                        break;
                    }
                case ConditionOperator.IsNotIn:
                    {
                        if (isCollectionChild)
                            expresion = $"{childName}.Any(!@{_count}.{comparison}({collectionChildPropName}))";
                        else
                            expresion = $"!@{_count}.{comparison}({filter.Field})";
                        break;
                    }
                case ConditionOperator.Equal:
                case ConditionOperator.NotEqual:
                case ConditionOperator.LessThan:
                case ConditionOperator.LessThanOrEqual:
                case ConditionOperator.GreaterThan:
                case ConditionOperator.GreaterThanOrEqual:
                    {
                        if (isCollectionChild)
                        {
                            var csharpReadyComparison = comparison == "=" ? "==" : comparison;
                            expresion = $"{childName}.Any({collectionChildPropName} {csharpReadyComparison} @{_count} )";
                        }
                        else
                            expresion = $"{filter.Field.ToPascalCase()} {comparison} @{_count}";
                        break;
                    }
                case ConditionOperator.IsNull:
                    {
                        if (isCollectionChild)
                        {
                            expresion = $"{childName}.Any({collectionChildPropName}  == null)";
                        }
                        else
                            expresion = $"{filter.Field.ToPascalCase()} = null";
                        break;
                    }
                case ConditionOperator.IsNotNull:
                    {
                        if (isCollectionChild)
                        {
                            expresion = $"{childName}.Any({collectionChildPropName}  != null)";
                        }
                        else
                            expresion = $"{filter.Field.ToPascalCase()} != null";
                        break;
                    }
            }

            if (filter.NextFilter != null)
                return "( " + expresion + " " + filter.Logic + " " + filter.NextFilter.ToDynamicLinqExpression<T>(false) + " )";
            else
                return expresion;
        }

        /// <summary>
        /// Provide data array of dynamic linq
        /// </summary>
        /// <typeparam name="T">type of source</typeparam>
        /// <param name="filter">filter object</param>
        /// <param name="res">refrenced type of object list that will return list of data needed for dynamic linq</param>
        private static void ToDataExpression<T>(this EveFilter filter, ref List<object> res)
        {
            Type targetPropType;
            var joinField = filter.Field.Split('.');

            if (joinField.Length > 1)
            {
                //If property containes join sign

                //Direct prop name in T
                var directPropName = joinField.FirstOrDefault().ToPascalCase();

                //Child prop name in direct prop
                var childPropName = joinField.LastOrDefault().ToPascalCase();


                //type of direct prop
                PropertyInfo directProp = typeof(T).GetProperty(directPropName);

                //type of child prop 
                var childPropType = directProp.PropertyType.GetGenericArguments().Any() ?
                        directProp.PropertyType.GetGenericArguments()[0].GetProperty(childPropName).PropertyType :
                        directProp.PropertyType.GetProperty(childPropName).PropertyType;

                targetPropType = childPropType;
            }
            else
            {
                //If property does not containes any join sign
                targetPropType = TypeExtensions.GetPropertyType<T>(filter.Field.ToPascalCase());
            }

            if (filter.Operator == ConditionOperator.IsIn
                || filter.Operator == ConditionOperator.IsNotIn)
            {
                //Generate a list of passed comma delimited values
                //The list should be of target property type

                //Creating type of List<target property type>
                Type listType = typeof(List<>).MakeGenericType(targetPropType);
                //Instantiate the List<target property type>
                var values = Activator.CreateInstance(listType) as IList;

                var valueStr = Convert.ToString(filter.Value);
                foreach (var item in valueStr.Split(','))
                {
                    values.Add(TypeExtensions.ChangeToNotNullType(item, targetPropType));
                }

                res.Add(values);
            }
            else
            {
                //Converting type of the passed parametere to the type of the target property
                //And adding the parameter to the param list

                res.Add(TypeExtensions.ChangeToNotNullType(filter.Value, targetPropType));
            }

            filter.NextFilter?.ToDataExpression<T>(ref res);
        }

        public static IQueryable<TEntity> ToSqlServerQueryable<TEntity>(this EveFilterSet filterset, DbSet<TEntity> dbSet) where TEntity : class
        {
            var lstLinqData = new List<object>();
            var firstTime = true;
            var lstLinqString = new List<string>();


            foreach (var filter in filterset.Filters)
            {
                lstLinqString.Add("(" + filter.ToDynamicLinqExpression<TEntity>(firstTime) + ")");
                firstTime = false;
                filter.ToDataExpression<TEntity>(ref lstLinqData);

            }
            var linqStringResult = string.Join(" " + filterset.Logic + " ", lstLinqString);

            for (int i = 0; i < lstLinqData.Count; i++)
            {
                var linqData = lstLinqData[i];
                linqData = linqData != null && (linqData is DateTime? || linqData is DateTime) ?
                    linqData = linqData.ToString() : linqData;
            }

            if (string.IsNullOrWhiteSpace(linqStringResult))
                return dbSet;
            else
                return dbSet.Where(linqStringResult, lstLinqData.ToArray());
        }

        public static IQueryable<TResult> PageNumberPaging<TResult>(this IQueryable<TResult> query, EvePageNumberPagination paging)
        {
            if (paging == null)
                paging = new EvePageNumberPagination();

            paging.PageNumber = paging.PageNumber <= 1 ? 1 : paging.PageNumber;
            var skip = (paging.PageNumber - 1) * paging.PageSize;
            var take = paging.PageSize;

            return query.Skip(skip).Take(take);
        }

        public static IQueryable<TResult> InfinitePaging<TResult>(this IQueryable<TResult> query, EveInfinityPagination paging)
        {
            if (paging == null)
                paging = new EveInfinityPagination();

            if (paging.FirstItemId > 0)
                query = query.Where($"Id > {paging.FirstItemId}");

            if (paging.LastItemId > 0)
                query = query.Where($"Id < {paging.LastItemId}");

            query = query.Take(paging.PageSize);

            return query;
        }

        public static IQueryable<TResult> AddPaging<TResult>(this IQueryable<TResult> query, EveBasePagination paging)
        {
            if (paging.Type == typeof(EvePageNumberPagination))
                return query.PageNumberPaging((EvePageNumberPagination)paging);
            else if (paging.Type == typeof(EveInfinityPagination))
                return query.InfinitePaging((EveInfinityPagination)paging);
            else
                return query;
        }
        public static IQueryable<TResult> ToJoined<TResult>(this IQueryable<TResult> query, List<string> joins)
        {
            foreach (var join in joins)
            {
                query = query.Include(join);
            }
            return query;
        }

        #region Include Method Extension






        public static IQueryable<T> Include<T>(this IQueryable<T> query, string include)
        {
            return query.Include(include.Split('.'));
        }

        public static IQueryable<T> Include<T>(this IQueryable<T> query, params string[] include)
        {
            var currentType = query.ElementType;
            var previousNavWasCollection = false;

            for (int i = 0; i < include.Length; i++)
            {
                var navigationName = include[i];
                var navigationProperty = currentType.GetProperty(navigationName);
                if (navigationProperty == null)
                {
                    throw new ArgumentException($"'{navigationName}' is not a valid property of '{currentType}'");
                }
                MethodInfo _include = typeof(EntityFrameworkQueryableExtensions).GetMethods()
                     .FirstOrDefault(m => m.Name == "Include");
                if (i == 0)
                {
                    var includeMethod = _include.MakeGenericMethod(query.ElementType, navigationProperty.PropertyType);

                    var expressionParameter = Expression.Parameter(currentType);
                    var expression = Expression.Lambda(
                        Expression.Property(expressionParameter, navigationName),
                        expressionParameter);

                    query = (IQueryable<T>)includeMethod.Invoke(null, new object[] { query, expression });
                }
                if (navigationProperty.PropertyType.GetInterfaces().Any(x => x.Name == typeof(ICollection<>).Name))
                {
                    previousNavWasCollection = true;
                    currentType = navigationProperty.PropertyType.GetGenericArguments().Single();
                }
                else
                {
                    previousNavWasCollection = false;
                    currentType = navigationProperty.PropertyType;
                }
            }

            return query;
        }
        #endregion

    }
}
