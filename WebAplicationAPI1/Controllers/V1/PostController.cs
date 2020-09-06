using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAplicationAPI1.Contracts.V1;
using WebAplicationAPI1.Contracts.V1.Requests;
using WebAplicationAPI1.Contracts.V1.Responses;
using WebAplicationAPI1.Domain;
using WebAplicationAPI1.Extentions;
using WebAplicationAPI1.Services;

namespace WebAplicationAPI1.Controllers.V1
{
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : Controller
    {
        #region Properties
        private readonly IPostService _postService;


        #endregion
        #region Contructors
        public PostController(IPostService postService)
        {
            _postService = postService;


        }

        #endregion
        #region Main Hanlders
        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _postService.GetAll_Async());
        }
        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postService.Get_Async(postId);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);

        }
        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
        {
            var userOwnsPost = await _postService.UserOwnsPos_Async(postId, HttpContext.GetUserId());
            if (!userOwnsPost)
            {
                return BadRequest(new
                {
                    Error = "You do not own this post"
                });
            }

            var post = await _postService.Get_Async(postId);
            post.Name = request.Name;
            var updated = await _postService.UpdatePost_Async(post);
            if (!updated)
            {
                return NotFound();
            }
            return Ok(post);

        }
        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var post = new Post();
            if (postRequest != null && !string.IsNullOrEmpty(postRequest.Name))
            {
                post.Name = postRequest.Name;
                post.UserId = HttpContext.GetUserId();
            }


            await _postService.Create_Async(post);
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());
            var response = new CreatePostResponse { Id = post.Id };
            return Created(locationUri, response);

        }
        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnsPost = await _postService.UserOwnsPos_Async(postId, HttpContext.GetUserId());
            if (!userOwnsPost)
            {
                return BadRequest(new
                {
                    Error = "You do not own this post"
                });
            }
            var deleted = await _postService.DeletePost_Async(postId);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
        #endregion


    }
}
