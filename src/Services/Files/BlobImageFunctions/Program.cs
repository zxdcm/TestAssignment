using BlobImageFunctions;
using BlobImageFunctions.Options;
using BlobImageFunctions.Services;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = new HostBuilder();
hostBuilder
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        if (hostContext.HostingEnvironment.IsDevelopment())
        {
            config.AddJsonFile("local.settings.json");
            config.AddUserSecrets<Program>();
        }
    });

hostBuilder.ConfigureServices((hostContext, services) =>
{
    services.AddAzureClients(clientsBuilder =>
    {
        var connectionString =
            hostContext.Configuration.GetSection("BlobStorage").GetValue<string>("ConnectionString");
            
        clientsBuilder
            .AddBlobServiceClient(connectionString)
            .WithName(BlobServiceClientDefaults.ClientName);
    });
    
    services.Configure<BlobStorageOptions>(
        hostContext.Configuration.GetSection("BlobStorage")
    );

    services.AddSingleton<IImageConverter, ImageConverter>();
});

var host = hostBuilder.Build();

await Startup.ProvisionDefaultsAsync(host.Services);

host.Run();
