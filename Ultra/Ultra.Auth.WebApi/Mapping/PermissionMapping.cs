using Mapster;
using Ultra.Auth.WebApi.DAL.Entities;
using Ultra.Auth.WebApi.Models;

namespace Ultra.Auth.WebApi.Mapping
{
    public class PermissionMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<Permission, PermissionModel>();
        }
    }
}
