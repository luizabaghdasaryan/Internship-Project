using WebApp.Shared.Models;

namespace WebApp.Shared.Repositories
{
    public interface IUserRepository
    {
        User? GetUserByEmail(string Email);
        void CreateUser(User user);
    }
}