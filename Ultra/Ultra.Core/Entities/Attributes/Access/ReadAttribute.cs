using Ultra.Core.Entities.Attributes.Access.Abstract;
using Ultra.Core.Models.Enums;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Core.Entities.Attributes.Access
{
    public class ReadAttribute : AccessAttributeBase
    {
        public ReadAttribute(bool isAvailable = true)
            : base(isAvailable, EntityMethod.Read)
        {
        }
    }
}
