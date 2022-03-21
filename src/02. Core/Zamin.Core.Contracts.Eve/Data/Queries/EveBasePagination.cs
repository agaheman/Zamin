namespace Zamin.Core.Contracts.Eve.Data.Queries
{
    /// <summary>
    /// Provides methodes and properties needed for Pagination
    /// </summary>
    public class EveBasePagination
    {
        public virtual Type Type { get; set; }
        /// <summary>
        /// Page Size in PageNumber Pagination
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// Total Count in PageNumber Pagination
        /// </summary>
        public long TotalCount { get; set; } = -1;

        /// <summary>
        /// Check Total search count should calculate or not.
        /// </summary>
        public virtual bool NeedTotalCount
        {
            get
            {
                return false;
            }
        }
    }
}
