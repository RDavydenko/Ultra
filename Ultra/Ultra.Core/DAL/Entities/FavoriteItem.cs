using Ultra.Core.Entities.Abstract;

namespace Ultra.Core.DAL.Entities
{
    // TODO: GUID_ENTITY
    public class FavoriteItem : EntityBase
    {
        public int UserId { get; set; }

        public int EntityTypeId { get; set; }
        public virtual EntityType EntityType { get; set; }

        public int? EntityId { get; set; }
    }
}
