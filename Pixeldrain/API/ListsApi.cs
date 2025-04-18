using System.Net.Http.Json;
using System.Text.Json;
using Pixeldrain.Models;

namespace Pixeldrain.API;

public class ListsApi : ApiBase
{
    public ListsApi(HttpClient httpClient) : base(httpClient)
    {
    }

    /// <summary>
    /// Retrieves all lists owned by the authenticated user from Pixeldrain.
    /// </summary>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A list of list metadata objects containing basic information about each list.</returns>
    /// <exception cref="PixeldrainException">Thrown when the API request fails or the user is not authenticated.</exception>
    public async ValueTask<List<ListInfoMetadata>> GetAllAsync(CancellationToken ct = default)
    {
        var res = await SendGetAsync<GetUserListsResponse>("user/lists", ct);
        return res.Lists;
    }

    /// <summary>
    /// Creates a new list on Pixeldrain with the specified files.
    /// </summary>
    /// <param name="title">The title for the new list.</param>
    /// <param name="files">A collection of file IDs to include in the list.</param>
    /// <param name="anonymous">Whether the list should be created anonymously or associated with the authenticated user.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>The ID of the newly created list.</returns>
    /// <exception cref="PixeldrainException">Thrown when the API request fails or the user is not authenticated (if not anonymous).</exception>
    public async ValueTask<string> CreateAsync(string title, IEnumerable<string> files, bool anonymous,
        CancellationToken ct = default)
    {
        var request = new CreateListRequest(title, anonymous, files);
        using var response =
            await Client.PostAsJsonAsync("list", request, JsonSerializerOptions.Web, ct);

        var data = await DeserializeOrThrow<ItemCreationResponse>(response, ct);
        return data.Id;
    }


    /// <summary>
    /// Retrieves detailed information about a specific list from Pixeldrain.
    /// </summary>
    /// <param name="id">The ID of the list to retrieve information for.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A list information object containing detailed metadata and files in the requested list.</returns>
    /// <exception cref="PixeldrainException">Thrown when the API request fails or the list doesn't exist.</exception>
    public async ValueTask<ListInfo> GetInfoAsync(string id, CancellationToken ct = default)
    {

        return await SendGetAsync<ListInfo>($"list/{id}", ct);
    }

    /// <summary>
    /// Updates an existing list on Pixeldrain with the specified title and files.
    /// </summary>
    /// <param name="id">The ID of the list to update.</param>
    /// <param name="title">The new title for the list.</param>
    /// <param name="files">A collection of file IDs to include in the updated list.</param>
    /// <param name="anonymous">Whether the list should be anonymous or associated with the authenticated user.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    /// <exception cref="PixeldrainException">Thrown when the API request fails, the list doesn't exist, or the user doesn't have permission to edit the list.</exception>
    public async ValueTask UpdateAsync(string id, string title, IEnumerable<string> files, bool anonymous,
        CancellationToken ct = default)
    {
        var request = new CreateListRequest(title, anonymous, files);
        using var response =
            await Client.PutAsJsonAsync($"list/{id}", request, JsonSerializerOptions.Web, ct);

        await VerifyOrThrow(response, ct);
    }

    /// <summary>
    /// Deletes a list from Pixeldrain by its ID.
    /// </summary>
    /// <param name="id">The ID of the list to delete.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    /// <exception cref="PixeldrainException">Thrown when the deletion fails, the list doesn't exist, or the user doesn't have permission to delete the list.</exception>
    public async ValueTask DeleteAsync(string id, CancellationToken ct = default)
    {
        using var response =
            await Client.DeleteAsync($"list/{id}", ct);

        await VerifyOrThrow(response, ct);
    }
}