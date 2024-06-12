using Dalapagos.Tunneling.Api.Endpoints;
using Dalapagos.Tunneling.Repository.EF;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEfTunnelingRepository(builder.Configuration);

var app = builder.Build();

app.RegisterServerEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();