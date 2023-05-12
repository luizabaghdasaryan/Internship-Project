using WebApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApp.Shared.Services;

namespace WebApp.Server.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService, IPasswordHasher<User> passwordHasher)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> LogIn(LoginUser user)
        {
            User? _user = await _userService.GetUserAndVerifyPassword(user);
            if (_user is not null)
            {
                return Ok(_userService.GenerateJWToken(_user));
            }
            return Unauthorized("User Not Found");
        }

        [HttpPost("createaccount")]
        public async Task<ActionResult> SignUp(User user)
        {
            if (await _userService.GetUserByEmail(user.Email) is not null) return Conflict("The email address you entered is already registered");
            await _userService.CreateUserWithHashedPassword(user);
            return Ok(_userService.GenerateJWToken(user));
        }
    }
}
