using Ultra.Core.Entities.Abstract;
using Ultra.Core.Models.Enums;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Auth.WebApi.DAL.Entities;

public class EntityAccess : EntityBase
{
    public string Entity { get; set; }
    public int? EntityId { get; set; }
    public EntityMethod Method { get; set; }
    public AccessType Type { get; set; }

    public int? UserId { get; set; }
    public virtual User User { get; set; }

    public int? RoleId { get; set; }
    public virtual Role Role { get; set; }
}

public enum AccessType
{
    /// <summary>
    /// Whitelist
    /// </summary>
    W,

    /// <summary>
    /// Blacklist
    /// </summary>
    B
}
