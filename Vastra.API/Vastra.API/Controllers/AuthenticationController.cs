using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        public async Task<ActionResult<string>> Authenticate(AuthenticationRequestBody authenticationRequestBody)
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
            claimsForToken.Add(new Claim("first_name", user.FirstName));
            claimsForToken.Add(new Claim("last_name", user.LastName?? ""));
            claimsForToken.Add(new Claim("phone", user.PhoneNumber));
            claimsForToken.Add(new Claim("email", user.EmailId ?? ""));

            _logger.LogDebug($"Claims set for user with phone number {user.PhoneNumber}. Claims : {claimsForToken.ToString()}");

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);
            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);
            _logger.LogDebug($"Successfully returning token : {tokenToReturn}");
            return Ok(tokenToReturn);
        }
    }
}
