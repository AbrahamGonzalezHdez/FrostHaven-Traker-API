using Application.Services.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Registration;

public static class AddApplicationServices
{
    public static IServiceCollection ApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CreateUserUseCase>();
        return services;
    }
}