namespace Zamin.Core.Domain.Eve.Entities
{
    public interface IEveIdentifiable
    {
        long Id { get; set; }
        Guid Guid { get; set; }
    }
}