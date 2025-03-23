using System.Runtime.Serialization;

namespace Pixeldrain;

public class PixeldrainException : Exception
{
    public string Code { get; }

    public PixeldrainException(string code) : base($"{code}: Unknown error")
    {
        Code = code;
    }
    

    public PixeldrainException(string? message, string code) : base($"{code}: {message}")
    {
        Code = code;
    }

    public PixeldrainException(string? message, Exception? innerException, string code) : base($"{code}: {message}", innerException)
    {
        Code = code;
    }
}