namespace BlobImageFunctions.Utils;

internal class BlobUtils
{
    public static (string containerName, string blobName) ParseBlobUrl(string url)
    {
        var blobUri = new Uri(url);
        var blobContainerName = blobUri.Segments[1].TrimEnd('/');
        var blobName = string.Join("", blobUri.Segments, 2, blobUri.Segments.Length - 2);

        return (blobContainerName, blobName);
    }
}