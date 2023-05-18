using System;
using Ultra.Core.Models.Enums;

namespace Ultra.Core.Entities.Attributes.Data.Methods.Abstract
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class MethodAttributeBase : Attribute
    {
        public bool IsAvailable { get; }
        public FieldMethods Method { get; }

        public MethodAttributeBase(bool isAvailable, FieldMethods method)
        {
            IsAvailable = isAvailable;
            Method = method;
        }
    }
}
