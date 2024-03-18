using Feed.Application.Commands.Comments;
using Feed.Core;
using Feed.Core.Cqrs.Command;
using Feed.Core.Entities;
using Feed.Core.Repositories;
using MediatR;

namespace Feed.Application.CommandHandlers.Comments;

internal class CreateCommentCommandHandler(
    ICommentRepository commentRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IIdentityContext identityContext
)
    : ICommandHandler<CreateCommentCommand, Unit>
{
    public async Task<Unit> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = request.PostId,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
            Content = request.Content,
            CreatorId = identityContext.IdentityId
        };

        await commentRepository.CreateAsync(comment, CancellationToken.None);
        await unitOfWork.CommitAsync();

        return Unit.Value;
    }
}