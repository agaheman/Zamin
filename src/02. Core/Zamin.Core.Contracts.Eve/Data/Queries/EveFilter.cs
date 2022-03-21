using static Zamin.Core.Contracts.Eve.Data.Queries.EveQueryEnums;

namespace Zamin.Core.Contracts.Eve.Data.Queries
{
    /// <summary>
    /// Represents a filter expression of Kendo DataSource.
    /// </summary>
    public class EveFilter
    {
        /// <summary>
        /// Gets or sets the name of the sorted field (property). Set to <c>null</c> if the <c>Filters</c> property is set.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the filtering operator. Set to <c>null</c> if the <c>Filters</c> property is set.
        /// </summary>
        public ConditionOperator Operator { get; set; }

        /// <summary>
        /// Gets or sets the filtering value. Set to <c>null</c> if the <c>Filters</c> property is set.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the filtering logic. Can be set to "or" or "and". Set to <c>null</c> unless <c>Filters</c> is set.
        /// </summary>
        public ConditionLogic Logic { get; set; } = ConditionLogic.And;

        /// <summary>
        /// Gets or sets the next filter expressions
        /// </summary>
        public EveFilter NextFilter { get; set; } = null;
    }
}