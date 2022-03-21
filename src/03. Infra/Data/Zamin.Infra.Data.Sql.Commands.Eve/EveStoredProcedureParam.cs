namespace Zamin.Infra.Data.Sql.Commands.Eve
{
    public class EveStoredProcedureParam
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }
    }
}