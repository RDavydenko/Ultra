using Ultra.Core.Entities.Abstract;
using Ultra.Core.Entities.States;

namespace Ultra.Auth.WebApi.DAL.Entities;

public class Permission : StatefulEntityBase<State>
{
    public string Entity { get; set; }
    public string Action { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<RolePermission> Roles { get; set; }
}
