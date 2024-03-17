using BlobImageFunctions.Services;
using FluentAssertions;
using FluentAssertions.Execution;
using SkiaSharp;

namespace BlobImageFunctions.Tests;

public class Tests
{
    [Test]
    public async Task ConvertAsyncValidInputReturnsImageInExpectedContentType()
    {
        // Arrange
        var sut = new ImageConverter();
        var expectedContentType = "image/jpeg";
        var sampleStream = GetSampleStream();
        
        // Act
        var resultStream = new MemoryStream();
        var resultMetadata = await sut.ConvertAsync(sampleStream, resultStream);

        // Assert
        var contentResult = resultStream.ToArray();
        using var skBitmap = SKBitmap.Decode(contentResult);
        using (new AssertionScope())
        {
            resultMetadata.Should().NotBeNull();
            resultMetadata.ContentType.Should().Be(expectedContentType);
            contentResult.Length.Should().BeGreaterThan(0);

            skBitmap.Width.Should().Be(600);
            skBitmap.Height.Should().Be(600);
        }
    }

    [Test]
    public void ConvertAsyncInvalidContentTypeThrowsConverterException()
    {
        // Arrange
        var sut = new ImageConverter();

        // Act
        var stream = new MemoryStream();
        var targetStream = new MemoryStream();
        stream.WriteByte(0);
        Action act = () => sut.ConvertAsync(stream, targetStream).GetAwaiter().GetResult();

        // Assert
        act.Should().Throw<ImageConverterException>();
    }

    private static Stream GetSampleStream()
    {
        var baseDirectory = TestContext.CurrentContext.TestDirectory;
        var filePath = Path.Combine(baseDirectory, "Data", "sample.png");

        return File.OpenRead(filePath);
    }
}