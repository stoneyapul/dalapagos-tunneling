namespace Dalapagos.Tunneling.Core;

public static class Constants
{
    public const string DevOpsBaseUrl = "https://dev.azure.com/dalapagos";
    public const string PipelineName = "dalapagos-tunneling-server-scripts";

    // Statuses map to http status codes.
    public const int StatusSuccess = 200;
    public const int StatusSuccessCreated = 201;
    public const int StatusSuccessAccepted = 202;
    public const int StatusSuccessNoResponse = 204;
    public const int StatusFailClient = 400;
    public const int StatusPermissionDenied = 403;
    public const int StatusFailServer = 500;

}
