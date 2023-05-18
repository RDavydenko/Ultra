using Ultra.Core.Entities.Attributes.Data.Methods.Abstract;
using Ultra.Core.Models.Enums;

namespace Ultra.Core.Entities.Attributes.Data.Methods
{
    public class UpdatedAttribute : MethodAttributeBase
    {
        public UpdatedAttribute(bool isAvailable = true)
            : base(isAvailable, FieldMethods.Updated)
        {
        }
    }
}
