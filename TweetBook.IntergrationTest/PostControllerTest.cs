using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAplicationAPI1.Contracts.V1;
using WebAplicationAPI1.Contracts.V1.Requests;
using WebAplicationAPI1.Contracts.V1.Responses;
using WebAplicationAPI1.Domain;
using Xunit;

namespace TweetBook.IntergrationTest
{
    public class PostControllerTest: IntegrationTest
    {
      
        [Fact]
        public async Task GetAll_WithPosts_ReturnsListPost()
        {
            // Arrange
           await Authentiction_Async();
            // Act
            var response = await _testClient.GetAsync(ApiRoutes.Posts.GetAll);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<Post>>()).Should().NotBeEmpty();

        }
        [Fact]
        public async Task Get_ReturnsPost_WhenPostExitsInDatabase()
        {
            //arrange
            await Authentiction_Async();
            var create = await CreatePost_Async(new CreatePostRequest {
                Name="Test1"
            });
            //act
            var response = await _testClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", create.Id.ToString()));
            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnPost = await response.Content.ReadAsAsync<Post>();
            returnPost.Id.Should().Be(create.Id);
            returnPost.Name.Should().Be("Test1");
        }
    }
}
