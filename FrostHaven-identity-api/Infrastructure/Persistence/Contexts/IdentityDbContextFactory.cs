using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Contexts;

public sealed  class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Development";

        // CurrentDirectory cuando corres el comando desde /Infrastructure:
        // C:\...\FrostHaven-identity-api\FrostHaven-identity-api\Infrastructure
        var currentDir = Directory.GetCurrentDirectory();

        // Subimos 1 nivel para llegar a la raíz:
        // C:\...\FrostHaven-identity-api\FrostHaven-identity-api
        var solutionRoot = Path.GetFullPath(Path.Combine(currentDir, ".."));

        // appsettings están en:
        // C:\...\FrostHaven-identity-api\FrostHaven-identity-api\APIs
        var apisPath = Path.Combine(solutionRoot, "APIs");

        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(apisPath, "appsettings.json"), optional: false)
            .AddJsonFile(Path.Combine(apisPath, $"appsettings.{environment}.json"), optional: true)
            .Build();

        // Usa el key “centralizado” que ya tengas
        var connectionString =
            config.GetConnectionString("Default")
            ?? config["Sql:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No se encontró la connection string. Revisa ConnectionStrings:Default o Sql:ConnectionString en APIs/appsettings*.json.");
        }

        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new IdentityDbContext(options);
    }
}