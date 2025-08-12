using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarAuction.Application.Common;
using CarAuction.Application.DTOs.Auth;
using CarAuction.Application.Services.Interfaces;
using CarAuction.Domain.Entities;
using CarAuction.Domain.Enums;

namespace CarAuction.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
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
                Balance = user.Balance
            });
        }


        public Task<ResponseResult<string>> LogOutAsync()
        {
            return Task.FromResult(ResponseResult<string>.SuccessResult("Logged out"));
        }

        public async Task<ResponseResult<string>> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUser != null)
                return ResponseResult<string>.FailResult("Username is already taken.");

            if (dto.Password != dto.ConfirmPassword)
                return ResponseResult<string>.FailResult("Passwords do not match.");

            var newUser = new ApplicationUser
            {
                UserName = dto.UserName,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreditStatus = CreditStatus.Active,
                Balance = dto.Balance
            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ResponseResult<string>.FailResult($"Registration failed: {errors}");
            }

            return ResponseResult<string>.SuccessResult(null, "Registration successful.");
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}