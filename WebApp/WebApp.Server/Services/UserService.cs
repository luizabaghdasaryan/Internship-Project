using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApp.Shared.Models;
using WebApp.Shared.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Shared.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserService(IUserRepository userRepository, IConfiguration configuration, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }
     
        public async Task<User?> GetUserAndVerifyPassword(LoginUser user)
        {
            User? _user = await Task.FromResult(_userRepository.GetUserByEmail(user.Email));
            if (_user is not null && _passwordHasher.VerifyHashedPassword(_user, _user.Password, user.Password) == PasswordVerificationResult.Success) 
            {
                return _user;
            }
            return null;
        }
        public async Task<User?> GetUserByEmail(string Email)
        {
            return await Task.FromResult(_userRepository.GetUserByEmail(Email));
        }
        public async Task CreateUserWithHashedPassword(User user)
        {
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            await Task.Run(() => _userRepository.CreateUser(user));
        }
        public string GenerateJWToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"), new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Role, user.Role) };
            var jwt = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: credentials);
            string token =  new JwtSecurityTokenHandler().WriteToken(jwt);
            return token;
        }
    }
}
