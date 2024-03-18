using System.ComponentModel.DataAnnotations;
using Feed.Core.Cqrs.Command;
using MediatR;

namespace Feed.Application.Commands.Comments;

public class CreateCommentCommand : ICommand<Unit>
{
    public required Guid PostId { get; set; }
    
    [MaxLength(100)]
    public required string Content { get; set; }
}