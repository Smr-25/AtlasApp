using Atlas.Application.Dtos.Users;
using Atlas.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IAccountService service) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var result = await service.RegisterAsync(dto);
        return !result.IsSuccess ? BadRequest(result) : Ok(result);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var result = await service.LoginAsync(dto);
        return !result.IsSuccess ? BadRequest(result) : Ok(result);
    }
}