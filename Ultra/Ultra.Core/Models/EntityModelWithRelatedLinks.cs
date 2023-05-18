using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultra.Core.Models
{
    public class EntityModelWithRelatedLinks<T>
    {
        public T? Entity { get; set; }
        public RelatedLink[] LinksToAddOrUpdate { get; set; } = Array.Empty<RelatedLink>();
        public RelatedLink[] LinksToDelete { get; set; } = Array.Empty<RelatedLink>();
    }
}
