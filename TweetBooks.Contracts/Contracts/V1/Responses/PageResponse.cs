using System;
using System.Collections.Generic;
using System.Text;

namespace TweetBooks.Contracts.Contracts.V1.Responses
{
    public class PageResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public PageResponse()
        {

        }
        public PageResponse(IEnumerable<T> data)
        {
            Data = data;
        }

        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }
    }
}
