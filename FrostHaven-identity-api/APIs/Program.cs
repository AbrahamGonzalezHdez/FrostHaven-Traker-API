using Application.Registration;
using Infrastructure.Registration;

namespace APIs;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        //------------Agregamos los servicios configurados en Application y en Infrastructure-----------
        builder.Services.ApplicationServices(builder.Configuration);
        builder.Services.InfrastructureServices(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.MapControllers(); 

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.Run();
    }
}