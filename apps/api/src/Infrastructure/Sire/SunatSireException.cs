namespace Infrastructure.Sire;

public sealed class SunatSireException(string message, int? statusCode = null, string? responseBody = null, Exception? innerException = null)
    : Exception(message, innerException)
{
    public int? StatusCode { get; } = statusCode;
    public string? ResponseBody { get; } = responseBody;
}
