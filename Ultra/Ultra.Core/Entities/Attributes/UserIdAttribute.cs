using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ultra.Core.Entities.Attributes.Meta.Abstract;
using Ultra.Core.Models.Meta;

namespace Ultra.Core.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UserIdAttribute : MetaAttributeBase
    {
        private readonly string _propertyName;

        public UserIdAttribute([CallerMemberName] string? propertyName = null)
        {
            _propertyName = propertyName!;
        }

        public override IEnumerable<MetaModel> GetMeta()
        {
            yield return new("foreignKey.path", _propertyName);
            yield return new("foreignKey.type", "User");
            yield return new("foreignKey.displayable", "UserName");
        }
    }
}
