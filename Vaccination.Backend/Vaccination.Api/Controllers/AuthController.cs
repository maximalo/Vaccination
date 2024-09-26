using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaccination.Application.Dtos.Authentication;
using Vaccination.Application.Interfaces;

namespace Vaccination.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var loginResult = await authService.LoginAsync(loginRequest);

            if (loginResult.IsSucceed)
            {
                return Ok(loginResult);
            }

            return BadRequest(loginResult);
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            var refreshResult = await authService.RefreshTokenAsync(tokenRequest);

            if (refreshResult.IsSucceed)
            {
                return Ok(refreshResult);
            }

            return BadRequest(refreshResult);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var registerResult = await authService.RegisterAsync(registerRequest);

            if (registerResult.IsSucceed)
            {
                return Ok(registerResult);
            }

            return BadRequest(registerResult);
        }
    }
}