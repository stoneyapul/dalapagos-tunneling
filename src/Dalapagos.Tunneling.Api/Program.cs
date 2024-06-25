using Dalapagos.Tunneling.Api.Endpoints;
using Dalapagos.Tunneling.Api.Security;
using Dalapagos.Tunneling.Core.DependencyInjection;
using Dalapagos.Tunneling.Repository.EF;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Dalapagos Tunnels API Documentation",
            Version = "v1",
            Description = "REST endpoints to manage tunneling."
        });
});

builder.Services.AddMediation();
builder.Services.AddEfTunnelingRepository(builder.Configuration);
builder.Services.AddEndpointSecurity(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.RegisterDeviceEndpoints();

app.UseSwagger();
app.UseReDoc(c =>
  {
    c.DocumentTitle = "Dalapagos Tunnels API Documentation";
    c.SpecUrl = "/swagger/v1/swagger.json";
  });   

app.Run();