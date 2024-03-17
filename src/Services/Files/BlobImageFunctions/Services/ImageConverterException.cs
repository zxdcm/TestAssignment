namespace BlobImageFunctions.Services;

public class ImageConverterException(string message, Exception ex) : Exception(message, ex);