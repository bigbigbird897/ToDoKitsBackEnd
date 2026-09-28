using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoKits.Models.Dtos;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Controllers;

/// <summary>账号认证接口：/api/auth（注册、登录）。</summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    public IUserService Users { get; set; } = null!;

    /// <summary>当前 JWT 密钥（属性注入配置）。</summary>
    public IConfiguration Config { get; set; } = null!;

    private string Secret => Config["Jwt:Secret"] ?? "ToDoKits-Dev-Key-Change-Me-2026!!";

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterInput input)
    {
        try
        {
            var user = await Users.RegisterAsync(input);
            return Ok(new AuthResult { Token = JwtHelper.CreateToken(user.Id, user.Username, Secret), User = new AuthUser { Id = user.Id, Username = user.Username } });
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginInput input)
    {
        var user = await Users.AuthenticateAsync(input.Username ?? "", input.Password ?? "");
        if (user == null) return Unauthorized(new { message = "用户名或密码错误" });
        return Ok(new AuthResult { Token = JwtHelper.CreateToken(user.Id, user.Username, Secret), User = new AuthUser { Id = user.Id, Username = user.Username } });
    }
}
