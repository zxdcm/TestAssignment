using System.ComponentModel.DataAnnotations;
using Feed.Core.Cqrs.Command;
using MediatR;

namespace Feed.Application.Commands.Posts;

public class CreatePostCommand : ICommand<Unit>
{
    [MaxLength(200)]
    public required string Caption { get; set; }
    
    public required Guid MediaBlobId { get; set; }
}