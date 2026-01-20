using Atlas.Application.Dtos.Users;
using Atlas.Application.Models;

namespace Atlas.Application.Services.Interfaces;

public interface IAccountService
{
    Task<ResponseModel<bool>> RegisterAsync(UserRegisterDto userRegisterDto);
    Task<ResponseModel<LoginResponseDto>> LoginAsync(UserLoginDto userLoginDto);
}