using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAplicationAPI1.Data;
using WebAplicationAPI1.Domain;
using WebAplicationAPI1.Options;

namespace WebAplicationAPI1.Services
{
    public class IdentityService : IIdentityService
    {
        #region Create Property
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _JwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _dataContext;

        #endregion

        #region Constructors
        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters, DataContext dataContext)
        {
            _userManager = userManager;
            _JwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _dataContext = dataContext;

        }


        #endregion


        #region functions
        private async Task <AuthenticationResult> GennerateAuthenticationResult_Async(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_JwtSettings.Secret);
            var claims = new List<Claim> {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim("id", user.Id)
                    };
            var userClaim = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaim);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_JwtSettings.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = new RefreshToken { 
                JwtId=token.Id,
                UserId=user.Id,
                CreationDate=DateTime.UtcNow,
                ExpiryDate=DateTime.UtcNow.AddMonths(6),
                Token= Guid.NewGuid().ToString()
        };
            await _dataContext.RefreshTokens.AddAsync(refreshToken);
            await _dataContext.SaveChangesAsync();
            return new AuthenticationResult
            {
                Token = tokenHandler.WriteToken(token),
                Success = true,
                RefreshToken=refreshToken.Token
            };
        }
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;

                }
                return principal;
            }
            catch
            {
                return null;
            }
        }
        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase) ;
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
            var newUserId = Guid.NewGuid();
            var newUser = new IdentityUser
            {
                Id=newUserId.ToString(),
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
            await _userManager.AddClaimAsync(newUser, new Claim("tags.view", "true"));
            return await GennerateAuthenticationResult_Async(newUser);

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
            return await GennerateAuthenticationResult_Async(exitstingUser);

        }

        public async Task<AuthenticationResult> RefreshToken_Async(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
            {
                return new AuthenticationResult {
                    Errors = new[] { "Invalid token" }
                };
            }
            var expiryDateunix = long.Parse(validatedToken.Claims.Single(x=>x.Type==JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                                                       .AddSeconds(expiryDateunix);
                                                      
            if(expiryDateTimeUtc> DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
            }
            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _dataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);
            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "This Refreshtoken not exist" } };
            }
            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
            }
            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };
            }
            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been Used" } };
            }
            if (storedRefreshToken.JwtId!=jti)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token dose  not match JWT" } };
            }
            storedRefreshToken.Used = true;
            _dataContext.RefreshTokens.Update(storedRefreshToken);
            await _dataContext.SaveChangesAsync();
            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GennerateAuthenticationResult_Async(user);
        }



        #endregion


    }


}
