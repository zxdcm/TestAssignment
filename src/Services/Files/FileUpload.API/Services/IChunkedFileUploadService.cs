namespace FileUpload.API.Services;

public interface IChunkedFileUploadService
{
    Task UploadChunkAsync(Guid blobId, Stream chunkStream, int chunkNumber);
    Task<bool> CommitUploadAsync(Guid blobId, string contentType, List<int> chunksIds);
}