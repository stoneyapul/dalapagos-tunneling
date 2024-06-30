namespace Dalapagos.Tunneling.Api.Mappers;

using Core.Model;

public abstract class MapperBase<TSrc, TDst>
{
    public abstract TDst Map(TSrc source);

    public OperationResult<TDst> MapOperationResult(OperationResult<TSrc> source)
    {
        if (source.Data is null)
        {
            return new OperationResult<TDst>(default, source.IsSuccessful, source.StatusCode, source.Errors);
        }
 
        return new OperationResult<TDst>(Map(source.Data), source.IsSuccessful, source.StatusCode, source.Errors);
    }

    public OperationResult MapOperationResult(OperationResult source)
    {
        return source;
    }
}