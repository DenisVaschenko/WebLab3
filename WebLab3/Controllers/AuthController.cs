using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WebLab3.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private OpenIdProperties openIdProperties;
        public AuthController(OpenIdProperties openIdProperties)
        {
            this.openIdProperties = openIdProperties;
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            Console.WriteLine("Login method called");
            var state = Guid.NewGuid().ToString();
            var authUrl = $"{openIdProperties.Authority}/login/oauth/authorize" +
                $"?client_id={openIdProperties.ClientId}" +
                $"&response_type=code" +
                $"&redirect_uri={openIdProperties.RedirectUri}" +
                $"&scope=openid profile phone";
            return Redirect(authUrl);
        }
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery]string code)
        {
            Console.WriteLine($"code is received: {code}");
            var tokenResponse = await ExchangeCodeForTokenAsync(code);
            Response.Cookies.Append("access_token", tokenResponse.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(10)
            });
            return Redirect("https://localhost:7275/index.html");
        }
        private async Task<TokenResponse> ExchangeCodeForTokenAsync(string code)
        {
            var client = new HttpClient();

            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", openIdProperties.RedirectUri },
                { "client_id", openIdProperties.ClientId },
                { "client_secret", openIdProperties.ClientSecret }
            };

            var content = new FormUrlEncodedContent(parameters);

            var response = await client.PostAsync($"{openIdProperties.Authority}/api/login/oauth/access_token", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TokenResponse>(json);
        }
    }
}
