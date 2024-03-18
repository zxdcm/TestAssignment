using Azure.Identity;
using Feed.Core;
using Feed.Core.Repositories;
using Feed.Infrastructure.Persistence;
using Feed.Infrastructure.Persistence.ModelBuilding;
using Feed.Infrastructure.Persistence.Options;
using Feed.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Feed.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        
        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var cosmosDbOptions = new CosmosDbOptions();
        configuration.GetSection("CosmosDb").Bind(cosmosDbOptions);
        services.AddDbContext<FeedContext>(options =>
        {
            options.UseCosmos(
                cosmosDbOptions.EndpointUrl,
                new DefaultAzureCredential(),
                cosmosDbOptions.DatabaseName
            );
        });
        
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
};