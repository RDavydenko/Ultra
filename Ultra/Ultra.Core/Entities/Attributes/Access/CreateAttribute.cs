using Ultra.Core.Entities.Attributes.Access.Abstract;
using Ultra.Core.Models.Enums;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Core.Entities.Attributes.Access
{
    public class CreateAttribute : AccessAttributeBase
    {
        public CreateAttribute(bool isAvailable = true)
            : base(isAvailable, EntityMethod.Create)
        {
        }
    }
}
