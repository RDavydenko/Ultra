using Ultra.Core.Entities.Attributes.Data.Methods.Abstract;
using Ultra.Core.Models.Enums;

namespace Ultra.Core.Entities.Attributes.Data.Methods
{
    public class PatchedAttribute : MethodAttributeBase
    {
        public PatchedAttribute(bool isAvailable = true)
            : base(isAvailable, FieldMethods.Patched)
        {
        }
    }
}
