namespace Dalapagos.Tunneling.Core.Behaviours;

using Model;

[AttributeUsage(AttributeTargets.Class)]
public sealed class CommandAuthorizationAttribute(AccessType accessType) : Attribute
{
    public AccessType AccessType
    {
        get {return accessType;}
    }
}