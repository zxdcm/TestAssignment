﻿namespace BlobImageFunctions.Options;

public class BlobStorageOptions
{
    public string DestinationContainer { get; set; } = null!;
    public int MaxFileSizeInBytes { get; set; }
    public string[] AllowedContentTypes { get; set; } = null!;
}