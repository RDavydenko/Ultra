using Ultra.Core.Entities.States;

namespace Ultra.Auth.WebApi.Models
{
    public class PermissionModel
    {
        public int Id { get; set; }
        public string Entity { get; set; }
        public string Action { get; set; }
        public State State { get; set; }
    }
}
