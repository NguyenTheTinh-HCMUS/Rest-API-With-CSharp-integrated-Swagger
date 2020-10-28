using System;
using System.Collections.Generic;
using System.Text;

namespace TweetBooks.Contracts.Contracts.V1.Responses
{
    public class Response<T>
    {
        public T Data { get; set; }

        public Response(T response)
        {
            Data = response;

        }
    }
}
