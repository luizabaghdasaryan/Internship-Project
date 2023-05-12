using WebApp.Shared.Models;

namespace WebApp.Shared.Services
{
    public interface IUserService
    { 
        Task<User?> GetUserAndVerifyPassword(LoginUser user);
        Task<User?> GetUserByEmail(string Email);
        Task CreateUserWithHashedPassword(User user);
        string GenerateJWToken(User user);
    }
}
