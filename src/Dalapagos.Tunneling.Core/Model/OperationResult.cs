namespace Dalapagos.Tunneling.Core.Model;

using System.Text.Json.Serialization;

public interface IOperationResult
{
    bool IsSuccessful { get; set; }
    string[] Errors { get; set; }
    int StatusCode { get; set; }
}

/// <summary>
/// Represents the result of an operation. This supports a result pattern , where the operation can be successful or not. 
///  i.e. Exceptions are not raised.
/// </summary>
public class OperationResult : IOperationResult
{
    public OperationResult()
    {
        IsSuccessful = true;
        Errors = [];
        StatusCode = 200;
    }

    public OperationResult(bool isSuccessful, int statusCode, string[] errors)
    {
        IsSuccessful = isSuccessful;
        Errors = errors;
        StatusCode = statusCode;
    }

    /// <summary>
    /// True if the operation was successful. Otherwise, false.
    /// </summary>
    [JsonPropertyName("isSuccessful")]
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// The status code. Maps to Http status codes.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    /// <summary>
    /// A list of errors or warnings.
    /// </summary>
    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[] Errors { get; set; }
}

/// <summary>
/// Represents the typed result of an operation. This supports a result pattern , where the operation can be successful or not. 
///  i.e. Exceptions are not raised.
/// </summary>
public class OperationResult<TResponse> : OperationResult
{
    public OperationResult(TResponse? data, bool isSuccessful, int statusCode, string[] errors)
    {
        IsSuccessful = isSuccessful;
        Errors = errors;
        StatusCode = statusCode;
        Data = data;
    }

    /// <summary>
    /// The data to be returned.
    /// </summary>
    public TResponse? Data { get; set; }
}