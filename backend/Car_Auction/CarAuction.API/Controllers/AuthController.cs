// using CarAuction.Application.Common;
// using CarAuction.Application.Common.Exceptions;
// using CarAuction.Application.DTOs.User;
// using CarAuction.Domain.Entities;
// using CarAuction.Domain.Enums;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using System.Text.RegularExpressions;
// using System.Security.Claims;

// [ApiController]
// [Route("api/[controller]")]
// public class AuthController : ControllerBase
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

//     public AuthController(UserManager<ApplicationUser> userManager)
//     {
//         _userManager = userManager;
//     }

//     [HttpPost("create")]
//     [AllowAnonymous] // This endpoint should allow anonymous access for registration
//     public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
//     {
//         if (!ModelState.IsValid)
//             throw new ValidationException("Invalid input data");

//         if (!Regex.IsMatch(request.Email, emailPattern))
//             throw new ValidationException("Invalid email format");

//         var user = new ApplicationUser
//         {
//             UserName = request.UserName,
//             Email = request.Email,
//             CreditStatus = CreditStatus.Active,
//             EmailConfirmed = true,
//             SecurityStamp = Guid.NewGuid().ToString(),
//             Balance = request.Balance >= 100000 ? request.Balance
//                 : throw new ValidationException("Balance must be at least 100.000"),
//         };

//         var result = await _userManager.CreateAsync(user, request.Password);

//         if (!result.Succeeded)
//         {
//             var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
//             throw new ValidationException(errorMessages);
//         }

//         return Ok(ResponseResult<string>.SuccessResult(null, "User created successfully."));
//     }

//     [HttpGet("profile")]
//     [Authorize] // This requires authentication
//     public async Task<IActionResult> GetProfile()
//     {
//         var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//         if (string.IsNullOrEmpty(userId))
//             return Unauthorized();

//         var user = await _userManager.FindByIdAsync(userId);
//         if (user == null)
//             return NotFound();

//         return Ok(ResponseResult<object>.SuccessResult(new
//         {
//             user.Id,
//             user.UserName,
//             user.Email,
//             user.Balance,
//             user.CreditStatus
//         }));
//     }
// }