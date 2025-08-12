using CarAuction.Application.Common;
using CarAuction.Application.DTOs.Auth;

namespace CarAuction.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseResult<LoginResultDto>> LoginAsync(LoginDto dto);
        Task<ResponseResult<string>> LogOutAsync();
        Task<ResponseResult<string>> RegisterAsync(RegisterDto dto);
    }
}