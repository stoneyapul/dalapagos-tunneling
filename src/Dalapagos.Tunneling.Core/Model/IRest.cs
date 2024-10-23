namespace Dalapagos.Tunneling.Core.Model;

using Refit;

public interface IRest
{
    [Get("/{**path}")]
    [QueryUriFormat(UriFormat.Unescaped)]
    Task<IApiResponse> Get(string path, CancellationToken cancellationToken);

    [Post("/{**path}")]
    [QueryUriFormat(UriFormat.Unescaped)]
    Task<IApiResponse> Post(string path, CancellationToken cancellationToken);

    [Put("/{**path}")]
    [QueryUriFormat(UriFormat.Unescaped)]
    Task<IApiResponse> Put(string path, CancellationToken cancellationToken);

    [Patch("/{**path}")]
    [QueryUriFormat(UriFormat.Unescaped)]
    Task<IApiResponse> Patch(string path, CancellationToken cancellationToken);

    [Delete("/{**path}")]
    [QueryUriFormat(UriFormat.Unescaped)]
    Task<IApiResponse> Delete(string path, CancellationToken cancellationToken);
}