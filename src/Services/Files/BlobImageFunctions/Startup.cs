using Azure.Storage.Blobs;
using BlobImageFunctions.Options;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BlobImageFunctions;

internal static class Startup
{
    public static async Task ProvisionDefaultsAsync(IServiceProvider sp)
    {
        var blobServiceClientFactory = sp.GetService<IAzureClientFactory<BlobServiceClient>>();
        var blobServiceClient = blobServiceClientFactory?.CreateClient(BlobServiceClientDefaults.ClientName);
        var options = sp.GetService<IOptions<BlobStorageOptions>>();
    
        var blobContainerClient = blobServiceClient!.GetBlobContainerClient(options!.Value.DestinationContainer);
        await blobContainerClient.CreateIfNotExistsAsync();
    }
}