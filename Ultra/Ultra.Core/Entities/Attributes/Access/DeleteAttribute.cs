using Ultra.Core.Entities.Attributes.Access.Abstract;
using Ultra.Core.Models.Enums;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Core.Entities.Attributes.Access
{
    public class DeleteAttribute : AccessAttributeBase
    {
        public DeleteAttribute(bool isAvailable = true)
            : base(isAvailable, EntityMethod.Delete)
        {
        }
    }
}
