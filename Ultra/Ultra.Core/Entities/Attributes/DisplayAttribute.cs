using System;

namespace Ultra.Core.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class DisplayAttribute : Attribute
    {
        public string DisplayName { get; }

        public string? DisplayableField { get; set; }

        public DisplayAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
