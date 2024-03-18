namespace FileUpload.API.Models;

public class CommitModelApi
{
    public Guid BlobId { get; set; }
    public string ContentType { get; set; }
    public List<int> ChunksNumbers { get; set; }
}