using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplicationAPI1.Contracts.V1;
using WebAplicationAPI1.Services;


namespace WebAplicationAPI1.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TagsController:Controller
    {
        private readonly IPostService _postService;
        #region Contructors
        public TagsController(IPostService postService)
        {
            _postService = postService;
        }
        #endregion

        #region Main Handlers
        [HttpGet(ApiRoutes.Tags.GetAll)]
        [Authorize(Policy = "TagViewer")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _postService.GetAllTags_Async());
        }
        #endregion
    }
}
