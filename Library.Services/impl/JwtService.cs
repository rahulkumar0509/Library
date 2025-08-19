using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Library.Services.Impl
{
    public class JwtService : IJwtService
    {
        IConfiguration _config;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _tokenSecret;
        ILogger<JwtService> _logger;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _config = configuration;
            _issuer = _config.GetSection("JWT").GetSection("Issuer").Value ?? "";
            _audience = _config.GetSection("JWT").GetSection("Audience").Value ?? "";
            _tokenSecret = _config.GetSection("JWT").GetSection("Key").Value ?? "";
            _logger = logger;
        }
        public string GenerateAccessToken(string UserId)
        {
            Claim[] claims = new Claim[] { //   claim is the info about user, usually json key value pair, will be used as subject in token
                new Claim("userId", UserId.ToString())
            };
            try
            {
                var tokenSecretKey = Encoding.ASCII.GetBytes(_tokenSecret); // convert string into byte array
                var securityTokenDescriptor = new SecurityTokenDescriptor() // properties of security token; jwt token
                {
                    TokenType = "Bearer",
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    Issuer = _config.GetSection("JWT").GetSection("Issuer").Value,
                    Audience = _config.GetSection("JWT").GetSection("Audience").Value,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenSecretKey), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(securityTokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{MethodBase.GetCurrentMethod().Name} : {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        public bool VerifyToken(string token)
        {
            return false;
        }
    }
}