namespace FileUpload.API.Options;

public class FileUploadOptions
{
    public int MaxFileSizeInBytes { get; set; }
    public int ChunkSizeInBytes { get; set; }
    public int MaxChunkNumber => MaxFileSizeInBytes / ChunkSizeInBytes;
    public HashSet<string> AllowedContentTypes { get; set; }
}