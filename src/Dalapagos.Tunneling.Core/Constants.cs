namespace Dalapagos.Tunneling.Core;

public static class Constants
{
    public const string DevOpsBaseUrl = "https://dev.azure.com/dalapagos";
    public const string PipelineName = "Rport Server";
    public const string FakeBaseUrl = "http://fake.dalapagos.com";

    // Default ports.
    public const ushort DefaultSshPort = 22;
    public const ushort DefaultHttpPort = 80;
    public const ushort DefaultHttpsPort = 443;

    // Suffixes for secret names.
    public const string TunnelingServerFingerprintNameSfx = "-Tnls-Finger";
    public const string TunnelingServerPassNameSfx = "-Tnls-RPortPass";
    public const string TunnelingServerVmPassNameSfx = "-Tnls-VmPass";

    // Statuses map to http status codes.
    public const int StatusSuccess = 200;
    public const int StatusSuccessCreated = 201;
    public const int StatusSuccessAccepted = 202;
    public const int StatusSuccessNoResponse = 204;
    public const int StatusFailClient = 400;
    public const int StatusPermissionDenied = 403;
    public const int StatusNotFound = 404;
    public const int StatusFailServer = 500;
    public const int StatusNotImplemented = 501;
}
