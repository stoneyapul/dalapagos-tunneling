using Dalapagos.Tunneling.Monitors;
using Dalapagos.Tunneling.Repository.EF;
using Dalapagos.Tunneling.Rport;
using Dalapagos.Tunneling.Secrets.KeyVault;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<HubsMonitor>();

builder.Services.AddKeyVaultSecrets();
builder.Services.AddEfTunnelingRepository(builder.Configuration);
builder.Services.AddRportTunneling();

var host = builder.Build();
host.Run();
