using ICMarkets.BlockchainDataAggregator.Application.Interfaces;
using ICMarkets.BlockchainDataAggregator.Application.Services;
using ICMarkets.BlockchainDataAggregator.Domain.Interfaces;
using ICMarkets.BlockchainDataAggregator.Infrastructure.Repositories;
using ICMarkets.BlockchainDataAggregator.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICMarkets.BlockchainDataAggregator.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dbName = configuration.GetConnectionString("DefaultConnection");
        var folder = Environment.CurrentDirectory;
        var dbPath = Path.Join(folder, dbName);

        services.AddDbContext<BlockchainDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));

        // we add the seeder as a scoped service because dbcontext is scoped as well
        services.AddScoped<BlockchainDbSeeder>();

        // Register Repositories
        services.AddScoped<IBlockchainRepository, BlockchainDataRepository>();

        // Register Application Services
        services.AddScoped<IBlockchainService, BlockchainService>();

        services.AddHttpClient();
    }
}
