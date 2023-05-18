using System;
using Ultra.Core.Models.Enums;
using Ultra.Infrastructure.Models.Enums;

namespace Ultra.Core.Entities.Attributes.Access.Abstract
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class AccessAttributeBase : Attribute
    {
        public bool IsAvailable { get; }

        public EntityMethod Method { get; }

        public AccessAttributeBase(bool isAvailable, EntityMethod method)
        {
            IsAvailable = isAvailable;
            Method = method;
        }
    }
}
