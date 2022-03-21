namespace Zamin.Core.Contracts.Eve.Data.Queries
{
    /// <summary>
    /// Infinity Pagination class. Inherits from BasePagination class
    /// </summary>
    public class EveInfinityPagination : EveBasePagination
    {
        /// <summary>
        /// First item id in infinity pagination
        /// </summary>
        public long FirstItemId { get; set; } = 0;

        /// <summary>
        /// Last item id in infinity pagination
        /// </summary>
        public long LastItemId { get; set; } = 0;

        /// <summary>
        /// Type in Infinit Pagination
        /// </summary>
        public override Type Type => typeof(EveInfinityPagination);
    }
}
