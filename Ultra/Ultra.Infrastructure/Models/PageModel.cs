namespace Ultra.Infrastructure.Models
{
    public class PageModel
    {
        public const int FirstPageNumber = 1;
        public const int PageSizeDefault = 20;
        public static readonly PageModel Default = new PageModel(FirstPageNumber, PageSizeDefault);

        public PageModel()
        {      
        }

        public PageModel(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }

        // TODO: скрыть из сваггера
        public int Offset => (PageNumber - 1) * PageSize;
    }
}
