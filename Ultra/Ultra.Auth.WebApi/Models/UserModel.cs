using Ultra.Core.Entities.States;

namespace Ultra.Auth.WebApi.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string UserName { get; set; }
        public State State { get; set; }
        public RoleModel[] Roles { get; set; }
        public PermissionModel[] Permissions { get; set; }
    }
}
