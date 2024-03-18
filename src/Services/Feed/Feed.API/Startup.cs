using Feed.API.Identity;
using Feed.Application;
using Feed.Core;
using Feed.Infrastructure;
using Feed.Infrastructure.Persistence.ModelBuilding;

namespace Feed.API;

internal class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IIdentityContext, IdentityIdentity>();

        services.AddSingleton(TimeProvider.System);

        services.AddApplication(Configuration);
        services.AddInfrastructure(Configuration);
        
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(WebApplication app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}
