using System;
using System.Collections.Generic;
using System.Text;

namespace TweetBooks.Contracts.Contracts.V1.Requests.querries
{
    public class PaginationQuerry
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PaginationQuerry(int pageNumber,int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        public PaginationQuerry()
        {
            PageNumber = 1;
            PageSize = 100;
        }

    }
}
