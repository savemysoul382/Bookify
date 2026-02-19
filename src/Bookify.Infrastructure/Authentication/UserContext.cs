using Bookify.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace Bookify.Infrastructure.Authentication;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId =>
        this._httpContextAccessor
            .HttpContext?
            .User
            .GetUserId() ??
        throw new ApplicationException("User context is unavailable");

    public string IdentityId =>
        this._httpContextAccessor
            .HttpContext?
            .User
            .GetIdentityId() ??
        throw new ApplicationException("User context is unavailable");
}