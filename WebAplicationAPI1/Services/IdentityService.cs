using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAplicationAPI1.Domain;
using WebAplicationAPI1.Options;

namespace WebAplicationAPI1.Services
{
    public class IdentityService : IIdentityService
    {
        #region Create Property
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _JwtSettings;

        #endregion

        #region Constructors
        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _JwtSettings = jwtSettings;

        }


        #endregion


        #region create Token
        private AuthenticationResult GennerateAuthenticationResult(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_JwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity
                (
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim("id", user.Id)
                    }
                ),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new AuthenticationResult
            {
                Token = tokenHandler.WriteToken(token),
                Success = true

            };
        }
        #endregion

        #region Main Handlers
        public async Task<AuthenticationResult> Register_Async(string email, string password)
        {
            var exitstingUser = await _userManager.FindByEmailAsync(email);
            if (exitstingUser != null)
            {
                return new AuthenticationResult { Errors = new[] { "User with this email address already exists" } };
            }
            var newUser = new IdentityUser
            {
                Email = email,
                UserName = email
            };
            var createUser = await _userManager.CreateAsync(newUser, password);
            if (!createUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createUser.Errors.Select(x => x.Description)
                };
            }
            return GennerateAuthenticationResult(newUser);

        }

        public async Task<AuthenticationResult> Login_Async(string email, string password)
        {
            var exitstingUser = await _userManager.FindByEmailAsync(email);
            if (exitstingUser == null)
            {
                return new AuthenticationResult { Errors = new[] { "User with this email address not exists" } };
            }
            var userHasValidPassword = await _userManager.CheckPasswordAsync(exitstingUser, password);
            if (!userHasValidPassword)
            {
                return new AuthenticationResult { Errors = new[] { "User/Password combination is wrong" } };

            }
            return GennerateAuthenticationResult(exitstingUser);

        }

        #endregion


    }


}
