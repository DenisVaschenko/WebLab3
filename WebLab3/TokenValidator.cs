using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

namespace WebLab3
{
    public class TokenValidator
    {
        private OpenIdProperties openIdProperties;
        public TokenValidator(OpenIdProperties openIdProperties)
        {
            this.openIdProperties = openIdProperties;
        }
        public async Task<JwtSecurityToken?> ValidateTokenAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            using var http = new HttpClient();
            var jwks = await http.GetFromJsonAsync<JsonWebKeySet>($"{openIdProperties.Authority}/.well-known/jwks");
            var parameters = new TokenValidationParameters
            {
                ValidIssuer = "http://localhost:8001",
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = jwks!.Keys
            };

            try
            {
                // Перевіряємо токен
                handler.ValidateToken(token, parameters, out var validatedToken);
                return (JwtSecurityToken)validatedToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
        }
    }
}
