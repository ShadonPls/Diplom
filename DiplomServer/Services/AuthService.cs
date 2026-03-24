using DiplomServer.Interfaces;
using DiplomServer.Interfaces.Repository;
using DiplomServer.Interfaces.Service;
using DiplomServer.Models;
using DiplomServer.Models.DTO.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DiplomServer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;

        public AuthService(IAuthRepository repository, IConfiguration config)
        {
            _repository = repository;
            _config = config;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _repository.GetByEmailAsync(email);
            if (user == null || !await _repository.ValidatePasswordAsync(user, password))
                throw new UnauthorizedAccessException("Неверный email или пароль");

            return GenerateJwtToken(user);
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var user = await _repository.CreateAsync(dto);
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("TeacherId", user.TeacherId?.ToString() ?? "0")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}
