using Application.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Postgres;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Registration;

public static class AddInfrastructureServices
{
    public static IServiceCollection InfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //-------Se obtiene la configuracion de la base de datos----------------------
        services.Configure<SqlOptions>(configuration.GetSection("Sql"));
        
        
        //-------Configuracion de conexion a BD---------------------------------------
        services.AddDbContextPool<SecurityPostgresDbContext>((sp, options) =>
        {
            var sql = sp.GetRequiredService<IOptions<SqlOptions>>().Value;

            if (string.IsNullOrWhiteSpace(sql.ConnectionString))
            {
                throw new InvalidOperationException("No se encontró 'Sql:ConnectionString'.");
            }

            options.UseNpgsql(sql.ConnectionString, npgsql =>
            {
                npgsql.CommandTimeout(sql.CommandTimeoutSeconds);
                // Si tienes migraciones en otro assembly, se especifica aquí:
                // npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                
            });

            // Recomendado en prod: evita tracking innecesario por default
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            
            

            // Solo para debugging en dev (si quieres condicionarlo por env):
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });
        
        return services;
    }
}