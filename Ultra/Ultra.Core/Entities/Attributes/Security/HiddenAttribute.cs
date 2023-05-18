using System;

namespace Ultra.Core.Entities.Attributes.Security
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HiddenAttribute : Attribute
    {
        public bool IsHidden { get; }

        public HiddenAttribute(bool hidden = true)
        {
            IsHidden = hidden;
        }
    }
}
