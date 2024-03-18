using Feed.Core;
using Feed.Core.Cqrs.Command;
using Feed.Core.Entities;
using Feed.Core.Repositories;
using MediatR;
using CreatePostCommand = Feed.Application.Commands.Posts.CreatePostCommand;

namespace Feed.Application.CommandHandlers.Posts;

internal class CreatePostCommandHandler(
    IPostRepository postRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IIdentityContext identityContext
)
    : ICommandHandler<CreatePostCommand, Unit>
{
    public async Task<Unit> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Caption = request.Caption,
            MediaBlobId = request.MediaBlobId,
            CreatorId = identityContext.IdentityId,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime
        };

        await postRepository.CreateAsync(post, CancellationToken.None);
        await unitOfWork.CommitAsync();

        return Unit.Value;
    }
}