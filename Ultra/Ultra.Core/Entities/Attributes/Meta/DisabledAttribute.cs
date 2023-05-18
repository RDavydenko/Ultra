using System;
using System.Collections.Generic;
using Ultra.Core.Entities.Attributes.Meta.Abstract;
using Ultra.Core.Models.Meta;
using Ultra.Core.Extensions;
using Ultra.Extensions;

namespace Ultra.Core.Entities.Attributes.Meta
{
    public class DisabledAttribute : MetaAttributeBase
    {
        public bool IsDisabled { get; set; }

        public DisabledAttribute(bool nullable = true)
        {
            IsDisabled = nullable;
        }

        public override IEnumerable<MetaModel> GetMeta()
        {
            return new MetaModel("disabled", IsDisabled).ToEnumerable();
        }
    }
}
