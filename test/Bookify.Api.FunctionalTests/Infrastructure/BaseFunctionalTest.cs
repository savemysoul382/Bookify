using System.Net.Http.Json;
using Bookify.Api.Controllers.Users;
using Bookify.Api.FunctionalTests.Users;
using Bookify.Application.Users.LogInUser;

namespace Bookify.Api.FunctionalTests.Infrastructure;

public abstract class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    protected readonly HttpClient httpClient;

    protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        this.httpClient = factory.CreateClient();
    }

    protected async Task<string> GetAccessTokenAsync()
    {
        HttpResponseMessage loginResponse = await this.httpClient.PostAsJsonAsync(
            "api/v1/users/login",
            new LogInUserRequest(
                UserData.RegisterTestUserRequest.Email,
                UserData.RegisterTestUserRequest.Password));

        var accessTokenResponse = await loginResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();

        return accessTokenResponse!.AccessToken;
    }
}
