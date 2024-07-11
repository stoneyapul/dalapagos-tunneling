namespace Dalapagos.Tunneling.Core.Exceptions;

using System;

[Serializable]
public class AccessDeniedException : Exception
{
    public AccessDeniedException() : base("Access denied")
    {
    }

    public AccessDeniedException(string message) : base(message)
    {
    }
}