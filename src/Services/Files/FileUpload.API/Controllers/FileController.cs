using FileUpload.API.Models;
using FileUpload.API.Options;
using FileUpload.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FileUpload.API.Controllers;

[Route("api/v1/files")]
public class FileController : ControllerBase
{
    private readonly IChunkedFileUploadService _chunkedFileUploadService;
    private readonly FileUploadOptions _fileUploadOptions;
    
    private const string ChunkNumberHeader = "ChunkNumber";
    private const string BlobIdHeader = "BlobId";

    public FileController(
        IChunkedFileUploadService chunkedFileUploadService, IOptions<FileUploadOptions> fileUploadOptions)
    {
        _chunkedFileUploadService = chunkedFileUploadService;
        _fileUploadOptions = fileUploadOptions.Value;
    }
    
    [HttpPost("init")]
    public IActionResult InitUpload()
    {
        // for now just id;
        // upload chunk / commit with this id should be matched against store (e.g redis)
        return Ok(
            new
            {
                BlobId = Guid.NewGuid(),
                ChunkSizeInBytes = _fileUploadOptions.ChunkSizeInBytes
            }
        );
    }
    
    [HttpPost("chunk")]
    public async Task<IActionResult> UploadChunk()
    {
        var validChunkNumber = int.TryParse(Request.Headers[ChunkNumberHeader], out var chunkNumber);
        var validBlob = Guid.TryParse(Request.Headers[BlobIdHeader], out var blobId);

        if (!validChunkNumber || !validBlob)
        {
            return BadRequest();
        }

        // underlying blob storage api requires length property
        // TODO: find a workaround to avoid buffering especially in memory
        var stream = new MemoryStream();
        await Request.Body.CopyToAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);
        
        await _chunkedFileUploadService.UploadChunkAsync(blobId, stream, chunkNumber);
            
        return Ok();
    }
    
    [HttpPost("commit")]
    public async Task<IActionResult> CommitBlob([FromBody] CommitModelApi commit)
    {
        if (!_fileUploadOptions.AllowedContentTypes.Contains(commit.ContentType))
        {
            return BadRequest();
        }
        
        await _chunkedFileUploadService.CommitUploadAsync(commit.BlobId, commit.ContentType, commit.ChunksNumbers);
 
        return Ok();
    }
}
