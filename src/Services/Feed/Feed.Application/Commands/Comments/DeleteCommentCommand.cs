using ErrorOr;
using Feed.Core.Cqrs.Command;
using MediatR;

namespace Feed.Application.Commands.Comments;

public class DeleteCommentCommand : ICommand<ErrorOr<Unit>>
{
    public required Guid CommentId { get; set; }
}

public static class DeleteCommentErrors
{
    public static Error NotFound => Error.Forbidden("Comment.NotFound", "");
    public static Error Forbidden => Error.Forbidden("Comment.Forbidden", "");
}