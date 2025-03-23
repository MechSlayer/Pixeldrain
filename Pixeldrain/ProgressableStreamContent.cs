using System.Net;

namespace Pixeldrain;

/// <summary>
/// Provides a specialized StreamContent that reports progress while streaming content to an HTTP request.
/// </summary>
/// <remarks>
/// This class enables tracking upload progress by reporting the number of bytes sent through an IProgress interface.
/// It is primarily used by the Files API to provide progress reporting during file uploads.
/// </remarks>
public class ProgressableStreamContent : StreamContent
{
    private const int DefaultBufferSize = 81920;

    private readonly int _bufferSize;
    private readonly IProgress<long> _progress;
    private readonly Stream _streamToWrite;
    private bool _contentConsumed;

    public ProgressableStreamContent(Stream streamToWrite, IProgress<long> progress, int bufferSize = DefaultBufferSize) : base(
        streamToWrite, bufferSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bufferSize);
        _bufferSize = bufferSize;
        _progress = progress;
        _streamToWrite = streamToWrite;
        _contentConsumed = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _streamToWrite.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override void SerializeToStream(Stream stream, TransportContext? context,
        CancellationToken cancellationToken)
    {
        PrepareContent();

        var buffer = new byte[_bufferSize];
        var uploaded = 0L;

        using (_streamToWrite)
        {
            while (true)
            {
                var length = _streamToWrite.Read(buffer);
                if (length <= 0)
                    break;
                
                uploaded += length;
                _progress.Report(uploaded);
                stream.Write(buffer.AsSpan(0, length));
            }
        }
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        return SerializeToStreamAsync(stream, context, CancellationToken.None);
    }

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context,
        CancellationToken cancellationToken)
    {
        PrepareContent();

        var buffer = new byte[_bufferSize];

        var uploaded = 0L;

        await using (_streamToWrite)
        {
            while (true)
            {
                var length = await _streamToWrite.ReadAsync(buffer, cancellationToken);
                if (length <= 0)
                    break;
                
                uploaded += length;
                _progress.Report(uploaded);
                await stream.WriteAsync(buffer.AsMemory(0, length), cancellationToken);
            }
        }
    }

    protected override bool TryComputeLength(out long length)
    {
        if (_streamToWrite.CanSeek)
        {
            length = _streamToWrite.Length;
            return true;
        }
        
        length = 0;
        return false;
    }


    private void PrepareContent()
    {
        if (!_contentConsumed)
        {
            _contentConsumed = true;
            return;
        }
    
        if (_streamToWrite.CanSeek)
            _streamToWrite.Position = 0;
        else
            throw new InvalidOperationException("The stream has already been read.");
    }
}