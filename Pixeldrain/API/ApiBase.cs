using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using Pixeldrain.Models;

namespace Pixeldrain.API;

public abstract class ApiBase
{
    /// <summary>
    /// Currently used <see cref="HttpClient"/>.
    /// </summary>
    protected HttpClient Client { get; }

    protected ApiBase(HttpClient httpClient)
    {
        Client = httpClient;
    }

    /// <summary>
    /// Attempts to deserialize the HTTP response content to the specified type or throws an exception if the response indicates an error.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response content to.</typeparam>
    /// <param name="message">The HTTP response message to process.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>The deserialized object of type T if successful, or null if the content couldn't be deserialized.</returns>
    /// <exception cref="PixeldrainException">Thrown when the HTTP response indicates an error, with details from the error response.</exception>
    protected static async ValueTask<T?> DeserializeOrThrowNull<T>(HttpResponseMessage message, CancellationToken ct = default)
    {
        if (message.IsSuccessStatusCode)
            return await message.Content.ReadFromJsonAsync<T>(JsonSerializerOptions.Web, ct);

        var error = await message.Content.ReadFromJsonAsync<ErrorResponse>(JsonSerializerOptions.Web, ct)
                    ?? new ErrorResponse("unknown", "Unknown error");

        throw new PixeldrainException(error.Message, error.Value);
    }

    
    /// <summary>
    /// Verifies that an HTTP response was successful or throws an exception with details from the error response.
    /// </summary>
    /// <param name="message">The HTTP response message to verify.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    /// <exception cref="PixeldrainException">Thrown when the HTTP response indicates an error, with details from the error response.</exception>
    protected static async ValueTask VerifyOrThrow(HttpResponseMessage message, CancellationToken ct = default)
    {
        if (message.IsSuccessStatusCode)
            return;

        var error = await message.Content.ReadFromJsonAsync<ErrorResponse>(JsonSerializerOptions.Web, ct)
                    ?? new ErrorResponse("unknown", "Unknown error");

        throw new PixeldrainException(error.Message, error.Value);
    }

    /// <summary>
    /// Attempts to deserialize the HTTP response content to the specified type or throws an exception if the response indicates an error or returns null.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response content to.</typeparam>
    /// <param name="message">The HTTP response message to process.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>The deserialized object of type T if successful.</returns>
    /// <exception cref="PixeldrainException">Thrown when the HTTP response indicates an error or returns null content.</exception>
    protected static async ValueTask<T> DeserializeOrThrow<T>(HttpResponseMessage message, CancellationToken ct = default)
    {
        var result = await DeserializeOrThrowNull<T>(message, ct);
        if (result is null)
            throw new PixeldrainException("Response was null", "null_response");

        return result;
    }

    
    /// <summary>
    /// Sends a GET request to the specified API endpoint and deserializes the response to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response content to.</typeparam>
    /// <param name="uri">The relative URI of the API endpoint.</param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>The deserialized response object of type T.</returns>
    /// <exception cref="PixeldrainException">Thrown when the API request fails or returns an error response.</exception>
    protected async ValueTask<T> SendGetAsync<T>([StringSyntax("Uri")] string uri, CancellationToken ct = default)
    {
        using var response = 
            await Client.GetAsync(uri, ct);
        
        var data = await DeserializeOrThrow<T>(response, ct);
        return data;
    }
}