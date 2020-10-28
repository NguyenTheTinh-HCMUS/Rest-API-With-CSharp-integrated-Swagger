using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBooks.Contracts.Contracts.V1.Requests.querries;
using WebAplicationAPI1.Contracts.V1;

namespace WebAplicationAPI1.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;
        public UriService(string baseUri)
        {
            _baseUri = baseUri;

        }
        public Uri GetAllPostsUri(PaginationQuerry pagination = null)
        {
            var uri = new Uri(_baseUri);
            if (pagination == null)
            {
                return uri;
            }
            var modifiedUri = QueryHelpers.AddQueryString(_baseUri, "pageNumber", pagination.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pagination.PageSize.ToString());
            return new Uri(modifiedUri);
        }

        public Uri GetPostUri(string PostId)
        {
            return new Uri(_baseUri + ApiRoutes.Posts.Get.Replace("{postId}", PostId));
        }
    }
}
