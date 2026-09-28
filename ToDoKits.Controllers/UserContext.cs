using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Controllers;

/// <summary>从当前 HTTP 请求的 JWT 身份读取 UserId（claim: NameIdentifier / sub）。</summary>
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _http;

    public UserContext(IHttpContextAccessor http) => _http = http;

    public long UserId
    {
        get
        {
            var principal = _http.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated != true) return 0;
            var v = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? principal.FindFirst("uid")?.Value;
            return long.TryParse(v, out var id) ? id : 0;
        }
    }
}
