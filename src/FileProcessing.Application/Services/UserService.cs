using FileProcessing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using FileProcessing.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FileProcessing.Application.Services
{
    public class UserService : IUserService
    {
        private readonly string _jwtKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _tokenExpiryMinutes;

        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _jwtKey = configuration["Jwt:Key"];
            _issuer = configuration["Jwt:Issuer"];
            _audience = configuration["Jwt:Audience"];
            _tokenExpiryMinutes = int.Parse(configuration["Jwt:TokenExpiryMinutes"] ?? "30");
        }

        public async Task<bool> RegisterUser(RegisterModel model)
        {
            var existingUser = await _userRepository.GetUserRecordAsync(new LoginModel { Username = model.Username });
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exists.");
            }

            await _userRepository.AddUserRecordAsync(model);
            return true;
        }

        public async Task<string> Authenticate(LoginModel model)
        {
            var response = await _userRepository.GetUserRecordAsync(model);

            if (response == null)
            {
                return null;  // Return null or custom error message
            }

            return GenerateJwtToken(model);
        }

        private string GenerateJwtToken(LoginModel user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Add roles or other claims as needed
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_tokenExpiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
