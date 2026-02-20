using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

internal sealed class AuthorizationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public AuthorizationService(ApplicationDbContext dbContext, ICacheService cacheService)
    {
        this._dbContext = dbContext;
        this._cacheService = cacheService;
    }

    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        string cacheKey = $"auth:roles-{identityId}";

        UserRolesResponse? cachedRoles = await this._cacheService.GetAsync<UserRolesResponse>(cacheKey);
        if (cachedRoles != null)
        {
            return cachedRoles;
        }

        UserRolesResponse roles = await this._dbContext.Set<User>()
            .Where(u => u.IdentityId == identityId)
            .Select(u => new UserRolesResponse
            {
                UserId = u.Id,
                Roles = u.Roles.ToList()
            })
            .FirstAsync();

        await this._cacheService.SetAsync(cacheKey, roles);

        return roles;
    }

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        string cacheKey = $"auth:permissions-{identityId}";

        HashSet<string>? cachedPermissions = await this._cacheService.GetAsync<HashSet<string>>(cacheKey);
        if (cachedPermissions != null)
        {
            return cachedPermissions;
        }

        ICollection<Permission> permissions = await this._dbContext.Set<User>()
            .Where(u => u.IdentityId == identityId)
            .SelectMany(u => u.Roles.Select(r => r.Permissions))
            .FirstAsync();

        HashSet<string> permissionsSet = permissions.Select(p => p.Name).ToHashSet();

        await this._cacheService.SetAsync(cacheKey, permissionsSet);

        return permissionsSet;
    }
}