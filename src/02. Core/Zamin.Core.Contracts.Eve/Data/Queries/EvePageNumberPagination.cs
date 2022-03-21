namespace Zamin.Core.Contracts.Eve.Data.Queries
{
    /// <summary>
    /// PageNumber Pagination class. Inherits from BasePagination class.
    /// </summary>
    public class EvePageNumberPagination : EveBasePagination
    {
        /// <summary>
        /// Page Number in PageNumber Pagination
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Type in PageNumber Pagination
        /// </summary>
        public override Type Type => typeof(EvePageNumberPagination);
        public override bool NeedTotalCount
        {
            get
            {
                return TotalCount < PageSize;
            }
        }
    }
}
