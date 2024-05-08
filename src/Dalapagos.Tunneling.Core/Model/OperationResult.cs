namespace Dalapagos.Tunneling.Core.Model;

public interface IOperationResult
{
    bool IsSuccessful { get; set; }
    string[] Errors { get; set; }
}

public class OperationResult : IOperationResult
{
    public OperationResult()
    {
        IsSuccessful = true;
        Errors = [];
    }

    public OperationResult(bool isSuccessful, string[] errors)
    {
        IsSuccessful = isSuccessful;
        Errors = errors;
    }

    public bool IsSuccessful { get; set; }
    public string[] Errors { get; set; }
}

public class OperationResult<TResponse>(TResponse? data, bool isSuccessful, string[] errors) : OperationResult(isSuccessful, errors)
{
    public TResponse? Data { get; set; } = data;
}