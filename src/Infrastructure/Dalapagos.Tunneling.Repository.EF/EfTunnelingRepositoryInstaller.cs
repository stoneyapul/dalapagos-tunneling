namespace Dalapagos.Tunneling.Repository.EF;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class EfTunnelingRepositoryInstaller
{
    public static void AddEfTunnelingRepository(this IServiceCollection services, IConfiguration? config)
    {
        ArgumentNullException.ThrowIfNull(config, nameof(config));

        services.AddDbContext<DalapagosTunnelsDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("TunnelsDb"));
        });

        services.AddScoped<Core.Infrastructure.ITunnelingRepository, EfTunnelingRepository>();
    }
}
