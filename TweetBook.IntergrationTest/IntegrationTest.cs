using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAplicationAPI1;
using WebAplicationAPI1.Contracts.V1;
using WebAplicationAPI1.Contracts.V1.Requests;
using WebAplicationAPI1.Contracts.V1.Responses;
using WebAplicationAPI1.Data;
using Xunit;
namespace TweetBook.IntergrationTest
{
    public class IntegrationTest
    {
        protected readonly HttpClient _testClient;
        //private readonly IServiceProvider _serviceProvider;
        public IntegrationTest()
        {
          
            var appFactory = new WebApplicationFactory<Startup>()
                                        .WithWebHostBuilder(builder=>
                                        {
                                            builder.ConfigureServices(services =>
                                            {
                                                services.RemoveAll(typeof(DataContext));
                                                services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Test_Database"));
                                            });
                                        });
            //_serviceProvider = appFactory.Services;
            _testClient = appFactory.CreateClient();
        }
        protected async Task Authentiction_Async()
        {
            _testClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await GetJwt_Async());
        }
        protected async Task<CreatePostResponse> CreatePost_Async(CreatePostRequest request)
        {
            var response = await _testClient.PostAsJsonAsync(ApiRoutes.Posts.Create, request);
            return await response.Content.ReadAsAsync<CreatePostResponse>();
        }
        private async Task<string> GetJwt_Async()
        {
            var response = await _testClient.PostAsJsonAsync(ApiRoutes.Identity.Login, new UserLoginRequest
            { 
                Email="abc10@gmail.com",
                Password="Abc.123"
            });
            var loginResponse = await response.Content.ReadAsAsync<AuthSuccessResopnse>();
            return loginResponse.Token;
        }
        //public void Dispose()
        //{
        //    using var serviceScope = _serviceProvider.CreateScope();
        //    var context = serviceScope.ServiceProvider.GetService<DataContext>();
        //    context.Database.EnsureDeleted();
        //}
    }
}
