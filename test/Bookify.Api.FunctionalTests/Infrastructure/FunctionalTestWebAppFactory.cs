using Bookify.Api.FunctionalTests.Users;
using Bookify.Application.Abstractions.Data;
using Bookify.Infrastructure;
using Bookify.Infrastructure.Authentication;
using Bookify.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Quartz;
using System.Net.Http.Json;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Bookify.Api.FunctionalTests.Infrastructure;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("bookify")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();

    private readonly KeycloakContainer _keycloakContainer = new KeycloakBuilder()
        .WithResourceMapping(
            new FileInfo(".files/bookify-realm-export.json"),
            new FileInfo("/opt/keycloak/data/import/realm.json"))
        .WithCommand("--import-realm")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            //  Удалим Quartz, если будут ошибки
            // services.RemoveAll(typeof(IHostedService));
            // services.RemoveAll(typeof(ISchedulerFactory));
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(this._dbContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention());

            services.RemoveAll(typeof(ISqlConnectionFactory));

            services.AddSingleton<ISqlConnectionFactory>(_ =>
                new SqlConnectionFactory(this._dbContainer.GetConnectionString()));

            services.Configure<RedisCacheOptions>(redisCacheOptions =>
                redisCacheOptions.Configuration = this._redisContainer.GetConnectionString());

            string? keycloakAddress = this._keycloakContainer.GetBaseAddress();

            // realms route in keycloak test container is different
            services.Configure<KeycloakOptions>(o =>
            {
                o.AdminUrl = $"{keycloakAddress}admin/realms/bookify/";
                o.TokenUrl = $"{keycloakAddress}realms/bookify/protocol/openid-connect/token";
            });

            services.Configure<AuthenticationOptions>(o =>
            {
                o.Issuer = $"{keycloakAddress}realms/bookify/";
                o.MetadataUrl = $"{keycloakAddress}realms/bookify/.well-known/openid-configuration";
            });
        });
    }

    public async Task InitializeAsync()
    {
        await this._dbContainer.StartAsync();
        await this._redisContainer.StartAsync();
        await this._keycloakContainer.StartAsync();

        await InitializeTestUserAsync();
    }

    public new async Task DisposeAsync()
    {
        await this._dbContainer.StopAsync();
        await this._redisContainer.StopAsync();
        await this._keycloakContainer.StopAsync();
    }

    private async Task InitializeTestUserAsync()
    {
        HttpClient httpClient = CreateClient();

        await httpClient.PostAsJsonAsync("api/v1/users/register", UserData.RegisterTestUserRequest);
    }
}