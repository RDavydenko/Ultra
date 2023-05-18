using System.Collections.Generic;
using Ultra.Core.Entities.Attributes.Meta.Abstract;
using Ultra.Core.Models.Meta;
using Ultra.Core.Extensions;
using Ultra.Extensions;

namespace Ultra.Core.Entities.Attributes.Meta
{
    public class RegexAttribute : MetaAttributeBase
    {
        public string Regex { get; }

        public RegexAttribute(string regex)
        {
            Regex = regex;
        }

        public override IEnumerable<MetaModel> GetMeta()
        {
            return new MetaModel("validation.regex", Regex).ToEnumerable();
        }
    }
}
