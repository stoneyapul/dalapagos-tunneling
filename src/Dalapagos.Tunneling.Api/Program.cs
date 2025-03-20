using System.Net;
using System.Reflection;
using System.Threading.RateLimiting;
using Dalapagos.Tunneling.Api.Endpoints;
using Dalapagos.Tunneling.Api.Security;
using Dalapagos.Tunneling.Core.DependencyInjection;
using Dalapagos.Tunneling.Monitor.HF;
using Dalapagos.Tunneling.Repository.EF;
using Dalapagos.Tunneling.Rport;
using Dalapagos.Tunneling.Secrets.KeyVault;
using Hangfire;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

// Add a rate limiter per IP address that allows up to 50 requests every 10 seconds.
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
    {
        var clientIp = context.Connection.RemoteIpAddress 
            ?? throw new InvalidOperationException("Client IP address is not available.");

        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 50,  
                Window = TimeSpan.FromSeconds(10),
                AutoReplenishment = true
            });
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Dalapagos.Tunneling.Core.xml"), includeControllerXmlComments: true);

    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Dalapagos Tunnels API Documentation",
            Version = "v1",
            Description = "ReST endpoints to manage tunneling."
        });
});

builder.Services.AddMediation();
builder.Services.AddKeyVaultSecrets();
builder.Services.AddEfTunnelingRepository(builder.Configuration);
builder.Services.AddHfMonitor(builder.Configuration);
builder.Services.AddRportTunneling();
builder.Services.AddEndpointSecurity(builder.Configuration);
 
var app = builder.Build();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.RegisterOrganizationEndpoints();
app.RegisterDeviceEndpoints();
app.RegisterHubEndpoints();
app.RegisterTunnelEndpoints();
app.RegisterRestEndpoints();

app.UseSwagger();
app.UseReDoc(c =>
{
  c.DocumentTitle = "Dalapagos Tunnels API Documentation";
  c.SpecUrl = "/swagger/v1/swagger.json";
});   

  app.UseHangfireDashboard("/monitor", new DashboardOptions
  {
      Authorization = [new HangfireAuthorizationFilter()]
  });

app.Run();