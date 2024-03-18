using ErrorOr;
using Feed.Application.Commands.Comments;
using Feed.Core;
using Feed.Core.Cqrs.Command;
using Feed.Core.Repositories;
using MediatR;

namespace Feed.Application.CommandHandlers.Comments;

internal class DeleteCommentCommandHandler(
    ICommentRepository commentRepository,
    IUnitOfWork unitOfWork,
    IIdentityContext identityContext
)
    : ICommandHandler<DeleteCommentCommand, ErrorOr<Unit>>
{
    public async Task<ErrorOr<Unit>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await commentRepository.GetByIdAsync(request.CommentId, cancellationToken);

        if (comment is null)
        {
            return DeleteCommentErrors.NotFound;
        }

        if (comment.CreatorId == identityContext.IdentityId)
        {
            await commentRepository.DeleteAsync(comment, CancellationToken.None);
            await unitOfWork.CommitAsync();
        }
        else
        {
            return DeleteCommentErrors.Forbidden;
        }

        return Unit.Value;
    }
}