namespace Ultra.Core.Services.Abstract
{
    public interface IUserAccessor
    {
        bool IsAuthenticated { get; }
        int UserId { get; }

        string[] GetRoles();

        (string Entity, string Action)[] GetPermissions();
    }
}
