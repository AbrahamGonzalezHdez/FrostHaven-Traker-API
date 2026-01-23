using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Postgres;
using Infrastructure.Repositories;
using Infrastructure.Security;
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
        
        //-------Repositorios---------------------------------------------------------
        services.AddScoped<IUserRepository, UserRepository>();
        
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
            });

            // Recomendado en prod: evita tracking innecesario por default
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            
            // Solo para debugging en dev (si quieres condicionarlo por env):
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });
        
        //-------Security------------------------------------------------------------
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();

        
        return services;
    }
}