using DataAccessLayer.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WeaselServicesAPI.Configuration;
using WeaselServicesAPI.Helpers.Interfaces;

namespace WeaselServicesAPI.Helpers.JWT
{
    public enum TokenType
    {
        Access = 1,
        Refresh = 2
    }

    public class TokenGenerator : ITokenGenerator
    {
        private readonly JWTSettings _settings;

        public TokenGenerator(JWTSettings settings)
        {
            _settings = settings;
        }

        public string GenerateToken(User u, TokenType type)
        {
            var symmetricKey = Convert.FromBase64String(_settings.Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Role, type.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, u.Uuid.ToString()),
                    new Claim(ClaimTypes.Name, u.Username),
                    new Claim(ClaimTypes.Email, u.Email)
                }),
                Expires = now.AddMinutes(Convert.ToInt32(GetExpirationTimeByTokenType(type))),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(symmetricKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public int ValidateToken(string token, bool isAccess, out string uuid)
        {
            uuid = null;

            // get principal and identity
            var simplePrinciple = GetPrincipal(token);
            var identity = simplePrinciple.Identity as ClaimsIdentity;

            // check identity
            if (identity == null || !identity.IsAuthenticated)
                return -1;

            // validate type of token
            var tokenType = isAccess ? TokenType.Access : TokenType.Refresh;
            var tokenClaim = identity.FindFirst(ClaimTypes.Role);
            if (tokenClaim?.Value != tokenType.ToString())
                return -2;

            // fetch uuid
            var uuidClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            uuid = uuidClaim?.Value;


            return !string.IsNullOrEmpty(uuid) ? 0 : -3;
        }

        private ClaimsPrincipal GetPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                throw new ApplicationException("JWT provided is null.");

            var symmetricKey = Convert.FromBase64String(_settings.Secret);

            var validationParameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
            };

            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

            return principal;
        }

        private int GetExpirationTimeByTokenType(TokenType type)
        {
            switch(type)
            {
                case TokenType.Access:
                    return _settings.AccessTokenExpirationTime;
                case TokenType.Refresh:
                    return _settings.RefreshTokenExpirationTime;
            }

            return 30;
        }
    }
}
