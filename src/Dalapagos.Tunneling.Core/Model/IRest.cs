namespace Dalapagos.Tunneling.Core.Model;

using Refit;

public interface IRest
{
    [Get("/{**path}")]
    [QueryUriFormat(UriFormat.Unescaped)]
    Task<HttpResponseMessage> Get(string path, CancellationToken cancellationToken);

    [Post("/{**path}")]
    [QueryUriFormat(UriFormat.Unescaped)]
    Task<HttpResponseMessage> Post(string path, CancellationToken cancellationToken);

    [Put("/{**path}")]
    [QueryUriFormat(UriFormat.Unescaped)]
    Task<HttpResponseMessage> Put(string path, CancellationToken cancellationToken);

    [Patch("/{**path}")]
    [QueryUriFormat(UriFormat.Unescaped)]
    Task<HttpResponseMessage> Patch(string path, CancellationToken cancellationToken);

    [Delete("/{**path}")]
    [QueryUriFormat(UriFormat.Unescaped)]
    Task<HttpResponseMessage> Delete(string path, CancellationToken cancellationToken);
}