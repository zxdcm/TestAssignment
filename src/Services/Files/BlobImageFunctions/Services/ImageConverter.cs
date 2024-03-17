using SkiaSharp;

namespace BlobImageFunctions.Services;

public class ImageConverter : IImageConverter
{
    private const int TargetSize = 600;
    private static readonly (string contentType, SKJpegEncoderOptions encoderOptions) TargetFormat =
        (
            "image/jpeg",
            new SKJpegEncoderOptions
            {
                Quality = 100
            }
        );

    public async Task<IImageConverter.ResultImageMetadata> ConvertAsync(Stream source, Stream target)
    {
        try
        {
            return await ConvertAsyncInternal(source, target, TargetSize);
        }
        catch (Exception ex) // intentionally; skiasharp has poor api
        {
            throw new ImageConverterException("Image resize has failed", ex);
        }
    }

    private async Task<IImageConverter.ResultImageMetadata> ConvertAsyncInternal(Stream source, Stream target, int targetSize)
    {
        using var skImage = SKBitmap.Decode(source);

        using var scaledBitmap = skImage.Resize(new SKImageInfo(targetSize, targetSize), SKFilterQuality.High);
        using var pixels = scaledBitmap.PeekPixels();
        using var encodedPixels = pixels.Encode(TargetFormat.encoderOptions);

        await encodedPixels.AsStream().CopyToAsync(target);

        return new IImageConverter.ResultImageMetadata
        {
            ContentType = TargetFormat.contentType
        };
    }
}