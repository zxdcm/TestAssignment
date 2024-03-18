namespace BlobImageFunctions.Options;

public class BlobStorageOptions
{
    public string DestinationContainer { get; set; } = null!;
    public int MaxFileSizeInBytes { get; set; }
    public HashSet<string> AllowedContentTypes { get; set; } = null!;
}