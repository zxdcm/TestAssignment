namespace Feed.Core.Entities;

public class Comment
{
    public required Guid Id { get; init; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required Guid CreatorId { get; set; }
    public required Guid PostId { get; set; }
}