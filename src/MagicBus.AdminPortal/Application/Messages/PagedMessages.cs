using System.Collections.Generic;

namespace MagicBus.AdminPortal.Application.Messages
{
    public class PagedMessages<T>
    {
        public int PageIndex { get; set; }
        
        public int PageSize { get; set; }
        
        public int TotalRecords { get; set; }

        public IEnumerable<T> Messages { get; set; }
    }
}