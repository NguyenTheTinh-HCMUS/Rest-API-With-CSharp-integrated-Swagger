using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBooks.Contracts.Contracts.V1.Requests.querries;

namespace WebAplicationAPI1.Services
{
    public interface IUriService
    {
        Uri GetPostUri(string PostId);
        Uri GetAllPostsUri(PaginationQuerry pagination = null);
    }
}
