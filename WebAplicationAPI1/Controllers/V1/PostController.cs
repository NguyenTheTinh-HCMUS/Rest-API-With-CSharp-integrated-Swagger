using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : Controller
    {
        #region Properties
        private readonly IPostService _postService;
        private readonly IMapper _mapper;


        #endregion
        #region Contructors
        public PostController(IPostService postService,IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;

        }

        #endregion
        #region Main Hanlders
        [HttpGet(ApiRoutes.Posts.GetAll)]
        [Authorize(Policy = "MustWorkForThetinh")]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAll_Async();
            return Ok(_mapper.Map<List<PostResponse>>(posts));
        }
        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postService.Get_Async(postId);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PostResponse>(post));

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
            Guid newID = Guid.NewGuid();
            if (postRequest != null && !string.IsNullOrEmpty(postRequest.Name))
            {
                post.Id = newID;
                post.Name = postRequest.Name;
                post.UserId = HttpContext.GetUserId();
                post.Tags = postRequest.Tags.Select(x => new PostTag { TagName = x.ToUpper(),PostId= newID }).ToList();
            }
            await _postService.Create_Async(post);
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());
            //var response = new PostResponse { Id = post.Id };
            return Created(locationUri, _mapper.Map<PostResponse>(post));

        }
        [HttpDelete(ApiRoutes.Posts.Delete)]
        [Authorize(Roles ="Admin",Policy = "MustWorkForThetinh")]
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
