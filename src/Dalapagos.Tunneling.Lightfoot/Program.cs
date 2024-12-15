using System.IdentityModel.Tokens.Jwt;
using Dalapagos.Tunneling.Core.DependencyInjection;
using Dalapagos.Tunneling.Monitor.HF;
using Dalapagos.Tunneling.Repository.EF;
using Dalapagos.Tunneling.Rport;
using Dalapagos.Tunneling.Secrets.KeyVault;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using Dalapagos.Tunneling.Lightfoot;
using BlazorComponentBus;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Add authentication with Microsoft identity platform.
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

builder.Services.AddScoped<ComponentBus>();
builder.Services.AddMediation();
builder.Services.AddKeyVaultSecrets();
builder.Services.AddEfTunnelingRepository(builder.Configuration);
builder.Services.AddHfMonitor(builder.Configuration);
builder.Services.AddRportTunneling();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
