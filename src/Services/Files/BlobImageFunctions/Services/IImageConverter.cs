namespace BlobImageFunctions.Services;

public interface IImageConverter
{
    public record ResultImageMetadata
    {
        public required string ContentType { get; init; }
    }

    Task<ResultImageMetadata> ConvertAsync(Stream source, Stream target); 
}