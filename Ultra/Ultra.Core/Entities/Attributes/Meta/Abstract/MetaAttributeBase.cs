using System;
using System.Collections.Generic;
using Ultra.Core.Models.Meta;

namespace Ultra.Core.Entities.Attributes.Meta.Abstract
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class MetaAttributeBase : Attribute
    {
        public abstract IEnumerable<MetaModel> GetMeta();
    }
}
