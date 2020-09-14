using Refit;
using System;
using System.Threading.Tasks;
using WebAplicationAPI1.Contracts.V1.Requests;

namespace TweetBook.Sdk.Sample
{
    class Program
    {
        static async Task WorkFlow_CRUD_Post()
        {
            var cathToken = string.Empty;
            var identityApi = RestService.For<IIdentityApi>("https://localhost:5001");
            var tweetBookApi = RestService.For<ITweetBookApi>("https://localhost:5001", new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(cathToken)
            });

            var loginResponse = await identityApi.Login_Async(new UserLoginRequest
            {
                Email = "admin@gmail.thetinh.com",
                Password = "Abc.123"
            });
            cathToken = loginResponse.Content.Token;
            var allPosts = await tweetBookApi.GetAll_Async();

            var createdPost = await tweetBookApi.Create_Async(new CreatePostRequest
            {
                Name = "Refit SDK",
                Tags = new[] { "Tag Refit" }
            });
            allPosts = await tweetBookApi.GetAll_Async();

            var updatedPost = await tweetBookApi.Update_Async(createdPost.Content.Id, new UpdatePostRequest
            {
                Name = "Refit SDk update"
            });
            allPosts = await tweetBookApi.GetAll_Async();
            await tweetBookApi.Delete_Async(updatedPost.Content.Id);
            allPosts = await tweetBookApi.GetAll_Async();
        }
        static async Task  Main(string[] args)
        {
            await WorkFlow_CRUD_Post();
        }
    }
}

