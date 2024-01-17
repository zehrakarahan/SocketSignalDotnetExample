using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocketSignalExample.Model;
using SocketSignalRExample.Hub;

namespace SocketSignalExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        public IAuthService _authService; 
        public HomeController(IAuthService authService)
        {
            _authService = authService;
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserModel login)
        {
            if (ModelState.IsValid && login != null)
            {
                var tokenString = _authService.GenerateJSONWebToken(login);
                return Ok(new { token = tokenString });
            }
            else
            {
                return BadRequest("userName ord Password null");
            }
        }

    }
}
