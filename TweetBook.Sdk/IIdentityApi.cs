using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAplicationAPI1.Contracts.V1;
using WebAplicationAPI1.Contracts.V1.Requests;
using WebAplicationAPI1.Contracts.V1.Responses;

namespace TweetBook.Sdk
{
    public interface IIdentityApi
    {
        [Post("/api/v1/identity/register/")]
        Task<ApiResponse<AuthSuccessResopnse>> Resgister_Async([Body] UserRegistrationRequest registrationRequest);

        [Post("/api/v1/identity/login/")]
        Task<ApiResponse<AuthSuccessResopnse>> Login_Async([Body] UserLoginRequest loginRequest);

        [Post("/api/v1/identity/refresh/")]
        Task<ApiResponse<AuthSuccessResopnse>> RefreshToken_Async([Body] RefreshTokenRequest refreshTokenRequest);



    }
}
