using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Vastra.API.Entities;
using Vastra.API.Models.Authentication;
using Vastra.API.Services;

namespace Vastra.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IVastraRepository _vastraRepository;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IConfiguration configuration, IVastraRepository vastraRepository, ILogger<AuthenticationController> logger){
            _configuration = configuration;
            _vastraRepository = vastraRepository;
            _logger = logger;
        }
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            _logger.LogDebug("Inside Authenticate in AuthenticationController");
            //Step 1: validate username/password
            var user = await _vastraRepository.ValidateUserCredentials(authenticationRequestBody.PhoneNumber,
                Hashing.GetSha256Hash(authenticationRequestBody.Password));
            if (user == null)
            {
                _logger.LogDebug("User with phoneNumber {0} and password {1} was not found.",
                    authenticationRequestBody.PhoneNumber, authenticationRequestBody.Password);
                return Unauthorized();
            }
            //Step 2: get role for user
            string roleName = (await _vastraRepository.GetRoleAsync(user.RoleId)).RoleName;
            _logger.LogDebug($"Token generation for role {roleName} in progress");
            //Step 3: create a token
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256 );
            //Step 4: claims
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim(ClaimTypes.Role, roleName));
            claimsForToken.Add(new Claim(ClaimTypes.Name, user.FirstName+" "+user.LastName));
            //claimsForToken.Add(new Claim("last_name", user.LastName?? ""));
            claimsForToken.Add(new Claim("phone", user.PhoneNumber));
            claimsForToken.Add(new Claim("email", user.EmailId ?? ""));

            _logger.LogDebug($"Claims set for user with phone number {user.PhoneNumber}." +
                $" Claims : {claimsForToken.ToString()}");

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(30),
                signingCredentials);
            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);
            //Step 5: create refresh token
            var refreshToken = GenerateRefreshToken();

            //Step 6: save refresh token data for user 
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpires = DateTime.Now.AddDays(1);
            user.DateModified = DateTime.Now;

            await _vastraRepository.SaveChangesAsync();

            //Step 7: construct token to return
            _logger.LogDebug($"Successfully returning token : {tokenToReturn}");
            return Ok(new 
            {
                AccessToken = tokenToReturn,
                RefreshToken = refreshToken,
                Expiration = jwtSecurityToken.ValidTo
            });

        }
        [HttpPost("refersh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel == null)
            {
                return BadRequest("Invalid Client Request");
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if(principal == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }
            string userPhone = principal.FindFirstValue("phone");

            var user = await _vastraRepository.GetUserByPhoneNumberAsync(userPhone);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpires <= DateTime.Now)
            {
                return BadRequest("Invalid accesstoken or refresh token");
            }
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);


            var newAccessToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                principal.Claims.ToList(),
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(30),
                signingCredentials);

            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.DateModified = DateTime.Now;

            await _vastraRepository.SaveChangesAsync();

            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler()
                .WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            });


        }
        [Authorize]
        [HttpPost]
        [Route("revoke/{userPhone}")]
        public async Task<IActionResult> Revoke(string userPhone)
        {
            var user = await _vastraRepository.GetUserByPhoneNumberAsync(userPhone);
            if(user == null)
            {
                return BadRequest("Invalid user phone number");
            }
            if (!await _vastraRepository.ValidateUserClaim(User, user.UserId))
            {
                _logger.LogDebug($"User claim failed for user with phone {userPhone} " +
                    $"in AuthenticationController.");
                return Forbid();
            }
            user.RefreshToken = null; 
            user.DateModified = DateTime.Now;

            await _vastraRepository.SaveChangesAsync();

            return NoContent();
        }
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"])),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                out SecurityToken securityToken);
            if(securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }
            return principal;

        }
    }
}
