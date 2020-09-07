using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplicationAPI1.Domain;

namespace WebAplicationAPI1.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> Register_Async(string email, string password);
        Task<AuthenticationResult> Login_Async(string email, string password);
        Task<AuthenticationResult> RefreshToken_Async(string token,string refreshToken);

    }
}
