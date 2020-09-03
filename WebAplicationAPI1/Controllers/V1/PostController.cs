using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAplicationAPI1.Contracts.V1;
using WebAplicationAPI1.Contracts.V1.Requests;
using WebAplicationAPI1.Contracts.V1.Responses;
using WebAplicationAPI1.Domain;
using WebAplicationAPI1.Services;

namespace WebAplicationAPI1.Controllers.V1
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService= postService;
            

        }
        [HttpGet(ApiRoutes.Posts.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(_postService.GetAll());
        }
        [HttpGet(ApiRoutes.Posts.Get)]
        public IActionResult Get([FromRoute] Guid postId)
        {
            var post = _postService.Get(postId);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);

        }
        [HttpPut(ApiRoutes.Posts.Update)]
        public IActionResult Update([FromRoute] Guid postId,[FromBody] UpdatePostRequest request)
        {
            var post = new Post() { 
            Id=postId,
            Name=request.Name
            };
            var updated = _postService.UpdatePost(post);
            if (!updated)
            {
                return NotFound(); 
            }
            return Ok(post);

        }
        [HttpPost(ApiRoutes.Posts.Create)]
        public IActionResult Create([FromBody] CreatePostRequest postRequest)
        {
            var post = new Post(); 
            if (postRequest== null || postRequest.Id == Guid.Empty)
            {
                post.Id = Guid.NewGuid();
            }
            else
            {
                post.Id = postRequest.Id;
            }

            _postService.GetAll().Add(post);
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}",post.Id.ToString());
            var response = new CreatePostResponse {Id=post.Id };
            return Created(locationUri,response);

        }
        [HttpDelete(ApiRoutes.Posts.Delete)]
        public IActionResult Delete([FromRoute] Guid postId)
        {
            var deleted = _postService.DeletePost(postId);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
