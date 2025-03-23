using Pixeldrain.Models;

namespace Pixeldrain.API;

public class UserApi : ApiBase
{
    public UserApi(HttpClient httpClient) : base(httpClient)
    {
    }

    /// <summary>
    /// Retrieves information about the currently authenticated user from Pixeldrain.
    /// </summary>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A user information object containing details about the authenticated user's account.</returns>
    /// <exception cref="PixeldrainException">Thrown when the API request fails or the user is not authenticated.</exception>
    public ValueTask<UserInfo> GetCurrentInfoAsync(CancellationToken ct = default)
    {
        return SendGetAsync<UserInfo>("user", ct);
    }


    /// <summary>
    /// Authenticates a user with Pixeldrain using username and password credentials.
    /// </summary>
    /// <param name="username">The username of the account to authenticate.</param>
    /// <param name="password">The password for the account.</param>
    /// <param name="appName">Optional name of the application making the request.</param>
    /// <returns>The authentication key (token) that can be used for subsequent API requests.</returns>
    /// <exception cref="PixeldrainException">Thrown when authentication fails due to invalid credentials or API errors.</exception>
    public async ValueTask<string> LoginAsync(string username, string password,
        string? appName = null)
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(username), "username");
        form.Add(new StringContent(password), "password");
        if (appName is not null)
            form.Add(new StringContent(appName), "app_name");

        using var response = await Client.PostAsync("user/login", form);
        var data = await DeserializeOrThrow<LoginResponse>(response);
        return data.AuthKey;
    }
}