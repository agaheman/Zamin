using System.Text;
using Zamin.Utilities.Eve.Extensions;
using static Zamin.Core.Contracts.Eve.Data.Queries.EveQueryEnums;

namespace Zamin.Infra.Data.Sql.Queries.Eve
{
    public class SortGenerator
    {
        private static IDictionary<SortDirection, string> SortDirectionDictionary = new Dictionary<SortDirection, string>
        {
            [SortDirection.Asc] = "",
            [SortDirection.Desc] = "descending"
        };

        public static string GetSortString(Dictionary<string, SortDirection> sorts)
        {

            var lstSort = new List<string>();

            foreach (var sort in sorts)
            {
                var sb = new StringBuilder();
                sb.Append(sort.Key.ToPascalCase());
                sb.Append(' ');
                sb.Append(SortDirectionDictionary[sort.Value]);

                lstSort.Add(sb.ToString());
            }

            return string.Join(",", lstSort);
        }
    }
}