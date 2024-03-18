using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using FileUpload.API.Options;
using Microsoft.Extensions.Options;

namespace FileUpload.API.Services;

public class ChunkedFileUploadService : IChunkedFileUploadService
{
    private readonly BlobServiceClient  _blobServiceClient;
    private readonly BlobStorageOptions _blobStorageOptions;
    private readonly FileUploadOptions _fileUploadOptions;
    private readonly ILogger<ChunkedFileUploadService> _logger;

    public ChunkedFileUploadService(
        BlobServiceClient blobServiceClient, 
        IOptions<BlobStorageOptions> blobStorageOptions, 
        IOptions<FileUploadOptions> fileUploadOptions,
        ILogger<ChunkedFileUploadService> logger
    )
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        _fileUploadOptions = fileUploadOptions.Value;
        _blobStorageOptions = blobStorageOptions.Value;
    }
    
    public async Task UploadChunkAsync(Guid blobId, Stream chunkStream, int chunkNumber)
    {
        _logger.LogInformation("Uploading chunk [{ChunkNumber}] for blob ID [{BlobId}].", chunkNumber, blobId);

        var blobContainer = GetBlobContainerClient();
        var blockBlobClient = blobContainer.GetBlockBlobClient(blobId.ToString());

        if (chunkNumber > _fileUploadOptions.MaxChunkNumber)
        {
            _logger.LogWarning(
                "Chunk number [{ChunkNumber}] exceeds the maximum allowed [{MaxChunkNumber}]. Blob ID: [{BlobId}]", 
                chunkNumber, _fileUploadOptions.MaxChunkNumber, blobId
            );
            // TODO: Blacklist user;
            return;
        }
        
        // TODO: add MD5 of chunk
        await blockBlobClient.StageBlockAsync(GetBlockId(chunkNumber), chunkStream);
        
        _logger.LogInformation("Uploaded chunk [{ChunkNumber}] for blob ID [{BlobId}].", chunkNumber, blobId);
    }

    public async Task<bool> CommitUploadAsync(Guid blobId, string contentType, List<int> chunksIds)
    {
        _logger.LogInformation("Committing upload for blob ID [{BlobId}] with [{ChunkCount}] chunks.", blobId, chunksIds.Count);

        var blobContainer = GetBlobContainerClient();
        var blockBlobClient = blobContainer.GetBlockBlobClient(blobId.ToString());

        if (chunksIds.Count > _fileUploadOptions.MaxChunkNumber)
        {
            _logger.LogWarning(
                "Number of chunks [{ChunkCount}] exceeds the maximum allowed [{MaxChunkNumber}]. Blob ID: [{BlobId}]", 
                chunksIds.Count, _fileUploadOptions.MaxChunkNumber, blobId
            );
            // TODO: Blacklist user;
            return false;
        }
        
        await blockBlobClient.CommitBlockListAsync(
            chunksIds.Select(GetBlockId), 
            new BlobHttpHeaders
            {
                ContentType = contentType
            }
        );

        _logger.LogInformation("Successfully committed upload for blob ID [{BlobId]}.", blobId);
        
        return true;
    }

    private BlobContainerClient GetBlobContainerClient()
    {
        return _blobServiceClient.GetBlobContainerClient(_blobStorageOptions.Container);
    }
    
    private string GetBlockId(int chunkNumber)
    {
        // For a given blob, the length of the value specified for the BlockId parameter must be the same size for each block.
        var maxLength = _fileUploadOptions.MaxChunkNumber.ToString().Length;
        var paddedNumber = chunkNumber.ToString().PadLeft(maxLength, '0');
        var base64BlockId = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(paddedNumber));

        return base64BlockId;
    }
}
