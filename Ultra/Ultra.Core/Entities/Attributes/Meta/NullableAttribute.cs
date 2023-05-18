using System;
using System.Collections.Generic;
using Ultra.Core.Entities.Attributes.Meta.Abstract;
using Ultra.Core.Models.Meta;
using Ultra.Core.Extensions;
using Ultra.Extensions;

namespace Ultra.Core.Entities.Attributes.Meta
{
    public class NullableAttribute : MetaAttributeBase
    {
        public bool IsNullable { get; set; }

        public NullableAttribute(bool nullable = true)
        {
            IsNullable = nullable;
        }

        public override IEnumerable<MetaModel> GetMeta()
        {
            return new MetaModel("validation.nullable", IsNullable).ToEnumerable();
        }
    }
}
