namespace Dalapagos.Tunneling.Core.Exceptions;

using System;

[Serializable]
public class DataNotFoundException : Exception
{
    public DataNotFoundException() : base("Data not found")
    {
    }

   public DataNotFoundException(string message) : base(message)
    {
    }
}
