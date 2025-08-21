using CarAuction.Application.Common;
using CarAuction.Application.DTOs.Auth;
using CarAuction.Application.Options;
using CarAuction.Application.Services.Interfaces;
using CarAuction.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace CarAuction.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOption _jwtOptions;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtOption> jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<ResponseResult<LoginResultDto>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return ResponseResult<LoginResultDto>.FailResult("Invalid credentials");

            var token = GenerateJwtToken(user);

            return ResponseResult<LoginResultDto>.SuccessResult(new LoginResultDto
            {
                Token = token,
                Username = user.UserName,
                Email = user.Email,
                UserId = user.Id,
                Balance = user.Balance
            });
        }

        public Task<ResponseResult<string>> LogOutAsync()
        {
            // For JWT tokens, logout is typically handled client-side by removing the token
            return Task.FromResult(ResponseResult<string>.SuccessResult("Logged out"));
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Balance", user.Balance.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}