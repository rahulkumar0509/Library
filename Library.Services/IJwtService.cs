// generate token method
// validate token method
// refresh token method
// what is access token ?
// SecurityTokenDescriptor vs JwtSecurityToken
using System.IdentityModel.Tokens.Jwt;

namespace Library.Services
{
    public interface IJwtService
    {
        public string GenerateAccessToken(string userId);
        public bool VerifyToken(string token);
    }
}