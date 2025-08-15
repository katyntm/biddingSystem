using CarAuction.Application.Common;
using CarAuction.Application.Common.Exceptions;
using CarAuction.Application.DTOs.User;
using CarAuction.Domain.Entities;
using CarAuction.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    public UserController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
    {
        if (!ModelState.IsValid)
            throw new ValidationException("Invalid input data");

        if (!Regex.IsMatch(request.Email, emailPattern))
            throw new ValidationException("Invalid email format");

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            CreditStatus = CreditStatus.Active,
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            Balance = request.Balance >= 100000 ? request.Balance
                : throw new ValidationException("Balance must be at least 100.000"),
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ValidationException(errorMessages);
        }

        return Ok(ResponseResult<string>.SuccessResult(null, "User created successfully."));
    }
}
