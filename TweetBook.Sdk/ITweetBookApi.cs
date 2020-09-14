using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAplicationAPI1.Contracts.V1.Requests;
using WebAplicationAPI1.Contracts.V1.Responses;

namespace TweetBook.Sdk
{
    [Headers("Authorization: Bearer")]
    public interface ITweetBookApi
    {
        [Get("/api/v1/posts/")]
        Task<ApiResponse<List<PostResponse>>> GetAll_Async();

        [Get("/api/v1/posts/{postId}")]
        Task<ApiResponse<PostResponse>> Get_Async(Guid postId);

        [Post("/api/v1/posts/")]
        Task<ApiResponse<PostResponse>> Create_Async([Body] CreatePostRequest createPostRequest);

        [Put("/api/v1/posts/{postId}")]
        Task<ApiResponse<PostResponse>> Update_Async(Guid postId, [Body] UpdatePostRequest   updatePostRequest);

        [Delete("/api/v1/posts/{postId}")]
        Task<ApiResponse<PostResponse>> Delete_Async(Guid postId);

    }
}
