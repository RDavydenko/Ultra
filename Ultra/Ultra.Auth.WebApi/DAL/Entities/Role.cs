using Ultra.Core.Entities.Abstract;
using Ultra.Core.Entities.States;

namespace Ultra.Auth.WebApi.DAL.Entities;

public class Role : StatefulEntityBase<State>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<UserRole> Users { get; set; }
    public virtual ICollection<RolePermission> Permissions { get; set; }
}
