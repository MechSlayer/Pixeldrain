using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Pixeldrain.API;
using Pixeldrain.Models;
using FileInfo = Pixeldrain.Models.FileInfo;

namespace Pixeldrain;

public class PixeldrainClient : IDisposable
{
    private const string BaseUri = "https://pixeldrain.com/api/";

    private readonly HttpClient _httpClient;

    public FilesApi Files { get; }
    public ListsApi Lists { get; }
    public UserApi User { get; }
    
    public TimeSpan Timeout
    {
        get => _httpClient.Timeout;
        set => _httpClient.Timeout = value;
    }


    private string? _key;

    public string? Key
    {
        get => _key;
        set
        {
            _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(value)
                ? null
                : new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($":{value}")));
            _key = value;
        }
    }

    private bool _disposed;

    public PixeldrainClient() : this(null)
    {
        
    }
    
    public PixeldrainClient(string? apiKey) : this(apiKey, new HttpClient())
    {
    }

    public PixeldrainClient(string? apiKey, HttpClient httpClient) : this(apiKey, BaseUri, httpClient)
    {
    }

    public PixeldrainClient(string? apiKey, string baseUri, HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUri);
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(baseUri);
        Key = apiKey;
        Files = new FilesApi(_httpClient);
        Lists = new ListsApi(_httpClient);
        User = new UserApi(_httpClient);
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        
        if (disposing)
        {
            _httpClient.Dispose();
        }

        _disposed = true;
    }
}