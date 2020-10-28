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
using WebAplicationAPI1.cache;
using TweetBooks.Contracts.Contracts.V1.Responses;
using TweetBooks.Contracts.Contracts.V1.Requests.querries;

namespace WebAplicationAPI1.Controllers.V1
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : Controller
    {
        #region Properties
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;


        #endregion
        #region Contructors
        public PostController(IPostService postService,IMapper mapper, IUriService uriService)
        {
            _postService = postService;
            _mapper = mapper;
            _uriService = uriService;

        }

        #endregion
        #region Main Hanlders
        [HttpGet(ApiRoutes.Posts.GetAll)]
        [Cached(600)]

        public async Task<IActionResult> GetAll([FromQuery]PaginationQuerry paginationQuerry)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuerry);
            var posts = await _postService.GetAll_Async(pagination);

            var postResponse = _mapper.Map<List<PostResponse>>(posts);

            if (paginationQuerry == null || paginationQuerry.PageNumber<1 || paginationQuerry.PageSize<1)
            {
                return Ok(new PageResponse<PostResponse>(postResponse));

            }
            var nextPage = pagination.PageNumber >= 1 ? _uriService.GetAllPostsUri(new PaginationQuerry { PageNumber = pagination.PageNumber+1, PageSize = pagination.PageSize }).ToString():null;
            var previousPage= pagination.PageNumber - 1 >= 1 ? _uriService.GetAllPostsUri(new PaginationQuerry { PageNumber = pagination.PageNumber -1, PageSize = pagination.PageSize }).ToString() : null;


            var paginationResponse = new PageResponse<PostResponse> { 
                Data=postResponse,
                PageNumber=pagination.PageNumber,
                PageSize=pagination.PageSize,
                NextPage=postResponse.Any() ? nextPage : null,
                PreviousPage=previousPage
            };
            return Ok(paginationResponse);
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        [Cached(600)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postService.Get_Async(postId);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(new Response<PostResponse>(_mapper.Map<PostResponse>(post)));

        }
        [Authorize(Policy = "MustWorkForThetinh")]
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
            return Ok( new Response<PostResponse>(_mapper.Map<PostResponse>(post)));

        }
        /// <summary>
        /// Create Post 
        /// </summary>
        /// <response code="201">Create the pots is success</response>
        /// <response code="400">Create the post is faild</response>

        [HttpPost(ApiRoutes.Posts.Create)]
        [ProducesResponseType(typeof(PostResponse),201)]
        [ProducesResponseType(typeof(ErrorResponse),400)]
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
            if(!(await _postService.Create_Async(post)))
            {
                return BadRequest(new ErrorResponse { Errors=new List<ErrorModel> { new ErrorModel { FieldName="",Message="Handler does not success"} } });

            }
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());
            //var response = new PostResponse { Id = post.Id };
            return Created(locationUri, new Response<PostResponse>(_mapper.Map<PostResponse>(post)));

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
