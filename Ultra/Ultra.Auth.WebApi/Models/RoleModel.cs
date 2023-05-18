using Ultra.Core.Entities.States;

namespace Ultra.Auth.WebApi.Models
{
    public class RoleModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public State State { get; set; }
    }
}
