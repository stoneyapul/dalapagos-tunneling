namespace Dalapagos.Tunneling.Core.Exceptions;

using System;

[Serializable]
public class ConfigurationException(string key) : Exception($"Configuration value for {key} was not found or invalid.")
{
}