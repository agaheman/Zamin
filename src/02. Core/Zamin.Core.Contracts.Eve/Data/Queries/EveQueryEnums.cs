namespace Zamin.Core.Contracts.Eve.Data.Queries
{
    public class EveQueryEnums
    {
        /// <summary>
        /// Enumarator for Order  
        /// </summary>
        public enum SortDirection
        {
            Asc,
            Desc
        }

        public enum ConditionLogic
        {
            And,
            Or
        }

        public enum DaType
        {
            SqlServer,
            ElasticSearch,
            Cassandra
        }

        public enum ActionType
        {
            Insert,
            Update,
            Delete,
            Select
        }

        public enum ConditionOperator
        {
            /// <summary>
            /// Equal operator 
            /// </summary>
            Equal,
            /// <summary>
            /// Not Equal operator
            /// </summary>
            NotEqual,
            /// <summary>
            /// Less Than operator
            /// </summary>
            LessThan,
            /// <summary>
            /// Less Than Or Equal operator
            /// </summary>
            LessThanOrEqual,
            /// <summary>
            /// Greater Than operator
            /// </summary>
            GreaterThan,
            /// <summary>
            /// Greater Than Or Equal operator
            /// </summary>
            GreaterThanOrEqual,
            /// <summary>
            /// In operator
            /// </summary>
            IsIn,
            /// <summary>
            /// Not In operator
            /// </summary>
            IsNotIn,
            /// <summary>
            /// Starts With function
            /// </summary>
            StartsWith,
            /// <summary>
            /// Ends With function
            /// </summary>
            EndsWith,
            /// <summary>
            /// Like function
            /// </summary>
            Like,
            /// <summary>
            /// Do Not Like function
            /// </summary>
            DoNotLike,
            /// <summary>
            /// Is Null Check
            /// </summary>
            IsNull,
            /// <summary>
            /// Is Not Null Check
            /// </summary>
            IsNotNull,
        }

        public enum AggregationFunction
        {
            Min,
            Max,
            Sum,
            Avrage,
            Count
        }
    }
}
