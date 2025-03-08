using Microsoft.IdentityModel.Tokens;
using ristorante_backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ristorante_backend.Services
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public int DurationInMinutes { get; set; }
    }

    public class JwtAuthenticationService
    {
        private readonly IConfiguration _configuration;
        public readonly JwtSettings _jwtSettings;
        private readonly UtenteService _utenteService;

        public JwtAuthenticationService(IConfiguration configuration, UtenteService utenteService)
        {
            _configuration = configuration;
            _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            _utenteService = utenteService;
        }

        public async Task<string> Authenticate(string email, string password)
        {
            Utente utente = await _utenteService.AuthenticateAsync(email, password);
            if (utente == null) { return null; }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email)
            };

            List<string> ruoli = await _utenteService.GetUserRolesAsync(utente.Id);
            foreach (string ruolo in ruoli)
            {
                claims.Add(new Claim(ClaimTypes.Role, ruolo));
            }

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] tokenKey = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}
