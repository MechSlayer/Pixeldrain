using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Pixeldrain.Models;
using FileInfo = Pixeldrain.Models.FileInfo;

namespace Pixeldrain.API;

public partial class FilesApi : ApiBase
{
    public const int MaxNameLength = 255;

    [GeneratedRegex(@"[/]", RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex InvalidNameRegex { get; }


    public FilesApi(HttpClient httpClient) : base(httpClient)
    {
    }

    /// <summary>
    /// Validates a file name according to Pixeldrain's file naming rules.
    /// </summary>
    /// <param name="name">The file name to validate.</param>
    /// <param name="exception">When validation fails, contains a <see cref="PixeldrainException"/> with the specific error; otherwise, null.</param>
    /// <returns>True if the name is valid; otherwise, false.</returns>
    /// <remarks>
    /// A valid file name must be no longer than 255 characters and must not contain forward slashes (/).
    /// </remarks>
    public static bool ValidateFileName(ReadOnlySpan<char> name,
        [NotNullWhen(false)] out PixeldrainException? exception)
    {
        if (name.Length > MaxNameLength)
        {
            exception = new PixeldrainException("File Name is too long, Max 255 characters allowed.", "name_too_long");
            return false;
        }

        if (InvalidNameRegex.IsMatch(name))
        {
            exception = new PixeldrainException(
                "The file name contains one or more characters which are not allowed in file names",
                "name_contains_illegal_character");
            return false;
        }

        exception = null;
        return true;
    }


    /// <summary>
    /// Normalizes a file name according to Pixeldrain's file naming rules.
    /// </summary>
    /// <param name="name">The file name to normalize.</param>
    /// <returns>A normalized version of the file name with invalid characters replaced and length limited to maximum allowed size.</returns>
    /// <remarks>
    /// This method truncates file names longer than <see cref="MaxNameLength"/> characters and replaces any forward slashes (/) with underscores (_).
    /// </remarks>
    public static string NormalizeFileName(string name)
    {
        if (name.Length > MaxNameLength)
            name = name[..MaxNameLength];
        return InvalidNameRegex.Replace(name, "_");
    }

    /// <summary>
    /// Validates and encodes a file name for use in Pixeldrain API requests.
    /// </summary>
    /// <param name="name">The file name to validate and encode.</param>
    /// <returns>The URI-encoded file name that is safe for use in API requests.</returns>
    /// <exception cref="PixeldrainException">Thrown when the file name is invalid according to Pixeldrain's naming rules.</exception>
    /// <remarks>
    /// This method first validates the file name using <see cref="ValidateFileName"/> and then applies URI encoding
    /// to make it safe for inclusion in URLs.
    /// </remarks>
    private static string EncodeFileName(string name)
    {
        if (!ValidateFileName(name, out var exception))
            throw exception;

        return Uri.EscapeDataString(name);
    }

    /// <summary>
    /// Retrieves all files owned by the authenticated user from Pixeldrain.
    /// </summary>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A list of file information objects containing details about each file.</returns>
    /// <exception cref="PixeldrainException">Thrown when the API request fails or the user is not authenticated.</exception>
    public async ValueTask<List<FileInfo>> GetAllAsync(CancellationToken ct = default)
    {
        var res = await SendGetAsync<GetUserFilesResponse>("user/files", ct);
        return res.Files;
    }


    /// <summary>
    /// Retrieves information about a specific file from Pixeldrain.
    /// </summary>
    /// <param name="id">The ID of the file to retrieve information for.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A file information object containing details about the requested file.</returns>
    /// <exception cref="PixeldrainException">Thrown when the API request fails or the file doesn't exist.</exception>
    public ValueTask<FileInfo> GetInfoAsync(string id, CancellationToken ct = default)
    {
        return SendGetAsync<FileInfo>($"file/{id}/info", ct);
    }


    /// <summary>
    /// Renames a file on Pixeldrain.
    /// </summary>
    /// <param name="id">The ID of the file to rename.</param>
    /// <param name="name">The new name for the file. Will be validated and encoded.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    /// <exception cref="PixeldrainException">Thrown when the rename operation fails, the file doesn't exist, or the name is invalid.</exception>
    public async ValueTask RenameAsync(string id, string name, CancellationToken ct = default)
    {
        name = EncodeFileName(name);
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("rename"), "action");
        content.Add(new StringContent(name), "name");
        using var response = await Client.PostAsync($"file/{id}", content, ct);
        await VerifyOrThrow(response, ct);
    }

    /// <summary>
    /// Uploads a file to Pixeldrain with the specified name and content.
    /// </summary>
    /// <param name="name">The name of the file to upload. Will be validated and encoded.</param>
    /// <param name="stream">The content of the file to upload.</param>
    /// <param name="progress">Optional progress reporter that receives the number of bytes uploaded.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>The ID of the uploaded file.</returns>
    /// <exception cref="PixeldrainException">Thrown when the file name is invalid or when the upload fails.</exception>
    public async ValueTask<string> UploadAsync(string name, Stream stream, IProgress<long>? progress = null,
        CancellationToken ct = default)
    {
        name = EncodeFileName(name);

        using var content =
            progress is null ? new StreamContent(stream) : new ProgressableStreamContent(stream, progress);
        using var response = await Client.PutAsync($"file/{name}", content, ct);
        var data = await DeserializeOrThrow<ItemCreationResponse>(response, ct);
        return data.Id;
    }

    /// <summary>
    /// Downloads a file from Pixeldrain by its ID and writes it to the specified output stream.
    /// </summary>
    /// <param name="id">The ID of the file to download.</param>
    /// <param name="outStream">The stream to write the downloaded file contents to.</param>
    /// <param name="progress">Optional progress reporter that receives the number of bytes downloaded.</param>
    /// <param name="bufferSize">The size of the buffer used for downloading. Default is 81920 bytes (80KB).</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    /// <exception cref="PixeldrainException">Thrown when the download fails or the file doesn't exist.</exception>
    public async ValueTask DownloadAsync(string id, Stream outStream, IProgress<long>? progress = null,
        int bufferSize = 81920,
        CancellationToken ct = default)
    {
        using var response =
            await Client.GetAsync($"file/{id}", HttpCompletionOption.ResponseHeadersRead, ct);

        await VerifyOrThrow(response, ct);

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        if (progress is null)
        {
            await stream.CopyToAsync(outStream, bufferSize, ct);
            return;
        }

        var buffer = new byte[bufferSize];
        var downloaded = 0L;
        while (true)
        {
            var length = await stream.ReadAsync(buffer, ct);
            if (length <= 0)
                break;

            await outStream.WriteAsync(buffer.AsMemory(0, length), ct);
            downloaded += length;
            progress.Report(downloaded);
        }
    }

    /// <summary>
    /// Deletes a file from Pixeldrain by its ID.
    /// </summary>
    /// <param name="id">The ID of the file to delete.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    /// <exception cref="PixeldrainException">Thrown when the deletion fails or the file doesn't exist.</exception>
    public async ValueTask DeleteAsync(string id, CancellationToken ct = default)
    {
        using var response =
            await Client.DeleteAsync($"file/{id}", ct);

        await VerifyOrThrow(response, ct);
    }
}