using WebApp.Shared.Models;

namespace WebApp.Client.Services
{
    public interface IUserService
    {
        Task<bool> LoggedInAndTokenStored(LoginUser user);
        Task<bool> RegisteredAndTokenStored(User user);
        Task LogOutAndRemoveToken();
    }
}
