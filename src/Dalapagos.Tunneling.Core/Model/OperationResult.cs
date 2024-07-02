namespace Dalapagos.Tunneling.Core.Model;

public interface IOperationResult
{
    bool IsSuccessful { get; set; }
    string[] Errors { get; set; }
    int StatusCode { get; set; }
}

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

    public bool IsSuccessful { get; set; }

    public int StatusCode { get; set; }

    public string[] Errors { get; set; }
}

public class OperationResult<TResponse> : OperationResult
{
    public OperationResult(TResponse? data, bool isSuccessful, int statusCode, string[] errors)
    {
        IsSuccessful = isSuccessful;
        Errors = errors;
        StatusCode = statusCode;
        Data = data;
    }

    public TResponse? Data { get; set; }
}