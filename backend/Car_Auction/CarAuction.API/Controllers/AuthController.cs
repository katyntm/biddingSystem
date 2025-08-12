using Microsoft.AspNetCore.Mvc;
using CarAuction.Application.DTOs.Auth;
using CarAuction.Application.Services.Interfaces;

namespace CarAuction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.LogOutAsync();
            return Ok(result);
        }
    }
}