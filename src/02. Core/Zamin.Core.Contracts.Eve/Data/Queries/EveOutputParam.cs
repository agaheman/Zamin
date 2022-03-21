using static Zamin.Core.Contracts.Eve.Data.Queries.EveQueryEnums;

namespace Zamin.Core.Contracts.Eve.Data.Queries
{
    /// <summary>
    /// This class provides a DTO as an Output parameter
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity</typeparam>
    /// <typeparam name="TPaging">Type of Paging</typeparam>
    public class EveOutputParam<TEntity, TPaging> where TPaging : EveBasePagination, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EveOutputParam{TEnity, TPagin}"/> class
        /// </summary>
        /// <param></param>
        public EveOutputParam()
        {
        }

        /// <summary>
        /// Result in type of <see cref="IEnumerable{TEntity}"/>
        /// </summary>
        public IEnumerable<TEntity> Result { get; set; }

        /// <summary>
        /// Paging in type of <see cref="TPaging"/>
        /// </summary>
        public TPaging Paging { get; set; } = new TPaging();

        /// <summary>
        /// FilterSet object that contains filter parameters
        /// </summary>
        public EveFilterSet FilterSet { get; set; } = new EveFilterSet();

        /// <summary>
        /// List of joined entity attributes
        /// </summary>
        public List<string> JoinList { get; set; } = new List<string>();

        /// <summary>
        /// OrderBy Dictionary in Type of <see cref="Dictionary{T,U}"/> where T , U are type of <see cref="string"/>
        /// </summary>
        public Dictionary<string, SortDirection> OrderByDictionary { get; set; } = new Dictionary<string, SortDirection>();
    }
}