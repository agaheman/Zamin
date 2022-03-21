namespace Zamin.Core.Domain.Eve.Entities
{
    public abstract class EveBaseEntity : IEveIdentifiable
    {
        public virtual long Id { get; set; }
        public virtual Guid Guid { get; set; } = Guid.Empty;

        public virtual long? CreatedBy { get; set; }
        public virtual DateTime? CreationDate { get; set; } = DateTime.Now;

        public virtual long? ModifiedBy { get; set; }
        public virtual DateTime? LastModifiedDate { get; set; } = DateTime.Now;
    }
}