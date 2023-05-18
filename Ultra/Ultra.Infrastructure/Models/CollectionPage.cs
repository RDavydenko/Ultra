using System.Collections;

namespace Ultra.Infrastructure.Models
{
    public class CollectionPage<T>
    {
        public List<T> Items { get; set; }
        public PageInfo PageInfo { get; set; }

        public CollectionPage(List<T> items, PageInfo pageInfo)
        {
            Items = items;
            PageInfo = pageInfo;
        }
    }

    public class PageInfo
    {
        public int Pages { get; set; }
        public int TotalCount { get; set; }
    }
}
