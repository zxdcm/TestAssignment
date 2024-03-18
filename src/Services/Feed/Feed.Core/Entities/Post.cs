namespace Feed.Core.Entities;

public class Post
{
    public required Guid Id { get; init; }
    public required string Caption { get; set; }
    public required Guid? MediaBlobId { get; init; }
    public required Guid CreatorId { get; set; }
    public required DateTime CreatedAt { get; set; } 
}