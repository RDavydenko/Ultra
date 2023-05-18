namespace Ultra.Auth.WebApi.DAL.Entities;

public class UserRole
{
    public int UserId { get; set; }
    public virtual User User { get; set; }

    public int RoleId { get; set; }
    public virtual Role Role { get; set; }

    public UserRole()
    {

    }

    public UserRole(User user, Role role)
    {
        User = user;
        UserId = user.Id;
        Role = role;
        RoleId = role.Id;
    }
}
