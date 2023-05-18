using Mapster;
using Ultra.Auth.WebApi.DAL.Entities;
using Ultra.Auth.WebApi.Models;

namespace Ultra.Auth.WebApi.Mapping
{
    public class UserMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<User, UserModel>()
                .Map(trg => trg.Roles, src => src.Roles.Select(x => x.Role.Adapt<RoleModel>()).ToArray())
                .Map(trg => trg.Permissions, src => src.Roles.SelectMany(x => x.Role.Permissions.Select(m => m.Permission.Adapt<PermissionModel>())).ToArray())
            ;
        }
    }
}
