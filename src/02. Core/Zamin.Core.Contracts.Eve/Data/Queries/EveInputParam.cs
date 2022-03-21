using static Zamin.Core.Contracts.Eve.Data.Queries.EveQueryEnums;

namespace Zamin.Core.Contracts.Eve.Data.Queries
{
    /// <summary>
    /// This class provides a DTO as an Input parameter
    /// </summary>
    /// <typeparam name="TEnity">Type of Entity</typeparam>
    /// <typeparam name="TPaging">Type of Paging</typeparam>
    public class EveInputParam<TEnity, TPaging> where TPaging : EveBasePagination, new()
    {
        /// <summary>
        /// Paging in type of <see cref="TPaging"/>
        /// </summary>
        public TPaging Paging { get; set; } = new TPaging();

        /// <summary>
        /// Predicate Dictionary in Type of <see cref="Dictionary{T,U}"/> where T , U are type of <see cref="string"/>
        /// </summary>
        //public Dictionary<string, string> PredicateDictionary { get; set; }


        /// <summary>
        /// FilterSet object that contains filter parameters
        /// </summary>
        public EveFilterSet FilterSet { get; set; } = new EveFilterSet();

        /// <summary>
        /// List of entity attributes to be joined
        /// </summary>
        public List<string> JoinList { get; set; } = new List<string>();

        /// <summary>
        /// Order Dictionary 
        /// </summary>
        public Dictionary<string, SortDirection> OrderByDictionary { get; set; } = new Dictionary<string, SortDirection>();
    }
}
