using Bookify.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application.IntegrationTests.Infrastructure;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    // нужно в Program запустить все миграции и заполнить тестовыми данными БД
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        this._scope = factory.Services.CreateScope();

        this.Sender = this._scope.ServiceProvider.GetRequiredService<ISender>();
        this.DbContext = this._scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
}