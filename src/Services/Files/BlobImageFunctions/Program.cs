using Azure.Identity;
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
        }
    });

hostBuilder.ConfigureServices((hostContext, services) =>
{
    var blobStorageSection = hostContext.Configuration.GetSection("BlobStorage");
    
    services.AddAzureClients(clientsBuilder =>
    {
        var blobStorageUri = blobStorageSection.GetValue<string>("Uri");
            
        clientsBuilder
            .UseCredential(new DefaultAzureCredential())
            .AddBlobServiceClient(new Uri(blobStorageUri))
            .WithName(BlobServiceClientDefaults.ClientName);
    });
    
    services.Configure<BlobStorageOptions>(blobStorageSection);

    services.AddSingleton<IImageConverter, ImageConverter>();
});

var host = hostBuilder.Build();

await Startup.ProvisionDefaultsAsync(host.Services);

host.Run();
