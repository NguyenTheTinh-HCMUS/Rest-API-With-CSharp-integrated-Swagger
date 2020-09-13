using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAplicationAPI1.Contracts.V1;
using WebAplicationAPI1.Contracts.V1.Requests;
using WebAplicationAPI1.Contracts.V1.Responses;
using WebAplicationAPI1.Services;

namespace WebAplicationAPI1.Controllers.V1
{
    public class IdentityController : Controller
    {
        #region Properties
        private readonly IIdentityService _identityService;
        #endregion
        #region Controtors
        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        #endregion
        #region Functions
        private AuthFailedResponse ValidFildeFormBody()
        {
            return new AuthFailedResponse {
                Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
            };
        }
        #endregion
        #region Main Handlers
        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ValidFildeFormBody());
            }
            var authResponse = await _identityService.Register_Async(request.Email, request.Password);
            if (authResponse == null || !authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });

            }
            return Ok(
                new AuthSuccessResopnse
                {
                    Token = authResponse.Token,
                    RefreshToken = authResponse.RefreshToken
                }
                );
        }
        /// <summary>
        ///  Login into application
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Faild</response>
        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ValidFildeFormBody());
            }
            var authResponse = await _identityService.Login_Async(request.Email, request.Password);
            if (authResponse == null || !authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });

            }
            return Ok(
                new AuthSuccessResopnse
                {
                    Token = authResponse.Token,
                    RefreshToken=authResponse.RefreshToken
                }
                );
        }
        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ValidFildeFormBody());
            }
            var authResponse = await _identityService.RefreshToken_Async(request.Token, request.RefreshToken);

            if (authResponse == null || !authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });

            }
            return Ok(
                new AuthSuccessResopnse
                {
                    Token = authResponse.Token,
                    RefreshToken = authResponse.RefreshToken
                }
                );

        }

        #endregion






    }
}
