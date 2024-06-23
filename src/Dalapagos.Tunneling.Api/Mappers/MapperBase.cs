namespace Dalapagos.Tunneling.Api.Mappers;

using Core.Model;

public abstract class MapperBase<TSrc, TDst>
{
    public abstract TDst Map(TSrc source);

    public OperationResult<TDst> MapOperationResult(OperationResult<TSrc> source)
    {
        ArgumentNullException.ThrowIfNull(source.Data, nameof(source.Data));
        return new OperationResult<TDst>(Map(source.Data), source.IsSuccessful, source.Errors);
    }

    public OperationResult MapOperationResult(OperationResult source)
    {
        return source;
    }
}