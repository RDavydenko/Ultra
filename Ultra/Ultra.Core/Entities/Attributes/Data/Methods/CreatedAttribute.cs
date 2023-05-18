using Ultra.Core.Entities.Attributes.Data.Methods.Abstract;
using Ultra.Core.Models.Enums;

namespace Ultra.Core.Entities.Attributes.Data.Methods
{
    public class CreatedAttribute : MethodAttributeBase
    {
        public CreatedAttribute(bool isAvailable = true)
            : base(isAvailable, FieldMethods.Created)
        {
        }
    }
}
