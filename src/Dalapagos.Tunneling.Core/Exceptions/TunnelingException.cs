namespace Dalapagos.Tunneling.Core.Exceptions;

using System;
using System.Net;

[Serializable]
public class TunnelingException : Exception
{
    public HttpStatusCode DownstreamStatusCode { get; init; } = HttpStatusCode.InternalServerError;

    public TunnelingException()
        : base("Tunnel exception.") { }

    public TunnelingException(string message)
        : base(message) { }

    public TunnelingException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        DownstreamStatusCode = statusCode;
    }
}
