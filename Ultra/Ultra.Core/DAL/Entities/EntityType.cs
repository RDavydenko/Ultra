using Ultra.Core.Entities.Abstract;

namespace Ultra.Core.DAL.Entities
{
    public class EntityType : EntityBase
    {
        public string SystemName { get; set; }
        public string? DisplayName { get; set; }
    }
}
