using ICMarkets.BlockchainDataAggregator.API.Middleware;
using ICMarkets.BlockchainDataAggregator.Infrastructure.Extensions;
using ICMarkets.BlockchainDataAggregator.Infrastructure.Seeders;

namespace ICMarkets.BlockchainDataAggregator.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHealthChecks();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                    .WithHeaders("GET")
                    .AllowAnyHeader();
            });
        });

        builder.Services.AddHttpLogging(logging =>
        {
            logging.CombineLogs = true;
        });

        builder.Services.AddInfrastructure(builder.Configuration);

        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpLogging();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseCors();

        app.MapHealthChecks("/healthcheckz");

        app.MapControllers();

        await BlockchainDbSeeder.SeedAsync(app.Services);

        app.Run();
    }
}
