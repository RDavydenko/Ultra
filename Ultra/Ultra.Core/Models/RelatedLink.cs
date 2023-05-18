using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultra.Core.Models
{
    public class RelatedLink
    {
        public int EntityId { get; set; }
        public string EntitySystemName { get; set; }
        public string EntityPropertyName { get; set; }
    }
}
