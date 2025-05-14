using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
namespace WebLab3.Controllers
{
    public class MainController : Controller
    {
        private TokenValidator tokenValidator;
        public MainController(TokenValidator tokenValidator)
        {
            this.tokenValidator = tokenValidator;
        }
        [HttpGet("userdata")]
        public async Task<IActionResult> GetUserData([FromHeader(Name = "Authorization")] string authHeader)
        {
            string token = Request.Cookies["access_token"] ?? Request.Headers["Authorization"].ToString().Split(' ').Last();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing.");
            }
            var validatedJwt = await tokenValidator.ValidateTokenAsync(token);
            if (validatedJwt == null)
                return Unauthorized("Invalid or expired token: " + token);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var username = jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value
                           ?? jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

            if (userId == null || username == null)
                return Unauthorized("Token is missing necessary claims");

            return Ok(new { userId, username });

        } 
    }
}
