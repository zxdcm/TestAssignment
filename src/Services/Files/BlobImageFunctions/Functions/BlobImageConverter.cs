using System.Text.Json;
using Azure.Messaging.EventGrid;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobImageFunctions.Options;
using BlobImageFunctions.Services;
using BlobImageFunctions.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlobImageFunctions.Functions;

public class BlobCreatedEventData
{
    public string Url { get; set; }
    public string ContentType { get; set; }
    public int ContentLength { get; set; }
}

public class BlobImageConverter(
    ILogger<BlobImageConverter> logger,
    IAzureClientFactory<BlobServiceClient> blobClientFactory,
    IOptions<BlobStorageOptions> blobStorageOptions,
    IImageConverter imageConverter
)
{
    const string StorageBlobCreatedEvent = "Microsoft.Storage.BlobCreated";

    private readonly BlobServiceClient _blobServiceClient = blobClientFactory.CreateClient(BlobServiceClientDefaults.ClientName);
    private readonly BlobStorageOptions _blobStorageOptions = blobStorageOptions.Value;
    private readonly HashSet<string> _allowedContentTypes = blobStorageOptions.Value.AllowedContentTypes.ToHashSet(StringComparer.OrdinalIgnoreCase);

    [Function(nameof(BlobImageConverter))]
    public async Task Run(
        [EventGridTrigger] EventGridEvent eventGridEvent
    )
    {
        logger.LogInformation("Event type: [{type}], Event subject: [{subject}]", eventGridEvent.EventType, eventGridEvent.Subject);
        
        if (!string.Equals(eventGridEvent.EventType, StorageBlobCreatedEvent, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("Event type is not [{EventType}]. Function execution is skipped.", StorageBlobCreatedEvent);
            return;
        }
        
        var blobEventData = eventGridEvent.Data.ToObjectFromJson<BlobCreatedEventData>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        (string containerName, string blobName) = BlobUtils.ParseBlobUrl(blobEventData.Url);
        
        if (blobEventData.ContentLength > _blobStorageOptions.MaxFileSizeInBytes 
            || !_allowedContentTypes.Contains(blobEventData.ContentType))
        {
            logger.LogWarning("Invalid blob [{blobName}]. [{ContentLength}] -- [{ContentType}].", blobName, blobEventData.ContentLength, blobEventData.ContentType);
            return;
            // todo: handle invalid file uploaded
        }
        
        logger.LogInformation("Processing [{blobName}]", blobName);
        
        var sourceBlobClient = GetSourceBlobClient(containerName, blobName);
        var targetBlobClient = GetTargetBlobClient(blobName);

        var metadata = await ConvertAsync(sourceBlobClient, targetBlobClient);
        
        await targetBlobClient.SetHttpHeadersAsync(
            new BlobHttpHeaders
            {
                ContentDisposition = "inline",
                ContentType = metadata.ContentType 
            }
        );
        
        logger.LogInformation("Image conversion completed [{containerName}] -- [{blobName}]", _blobStorageOptions.DestinationContainer, blobName);
    }

    private async Task<IImageConverter.ResultImageMetadata> ConvertAsync(BlobClient sourceBlobClient, BlobClient targetBlobClient)
    {
        await using var sourceStream = await sourceBlobClient.OpenReadAsync();
        await using var destinationStream = await targetBlobClient.OpenWriteAsync(true);
         
        var metadata = await imageConverter.ConvertAsync(sourceStream, destinationStream);
        return metadata;
    }

    private BlobClient GetSourceBlobClient(string blobContainerName, string blobName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
        
        var blobClient = blobContainerClient.GetBlobClient(blobName);
        
        return blobClient;
    }
    
    private BlobClient GetTargetBlobClient(string blobName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageOptions.DestinationContainer);
        var destinationBlobClient = blobContainerClient.GetBlobClient(blobName);
        return destinationBlobClient;
    }
}