using static Zamin.Core.Contracts.Eve.Data.Queries.EveQueryEnums;

namespace Zamin.Core.Contracts.Eve.Data.Queries
{
    public class EveFilterSet
    {
        public List<EveFilter> Filters { get; set; } = new List<EveFilter>();
        public ConditionLogic Logic { get; set; } = ConditionLogic.And;
    }
}