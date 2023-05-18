using Ultra.Core.Entities.Abstract;
using Ultra.Core.Entities.States;

namespace Ultra.Auth.WebApi.DAL.Entities;

public class User : StatefulEntityBase<State>
{
    public string Login { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }

    public virtual ICollection<UserRole> Roles { get; set; }
}
