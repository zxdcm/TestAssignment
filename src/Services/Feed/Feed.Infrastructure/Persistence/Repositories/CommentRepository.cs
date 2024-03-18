using Feed.Core.Entities;
using Feed.Core.Repositories;
using Feed.Infrastructure.Persistence.ModelBuilding;
using Microsoft.EntityFrameworkCore;

namespace Feed.Infrastructure.Persistence.Repositories;

public class CommentRepository(FeedContext feedContext) :  ICommentRepository
{
    private readonly FeedContext _feedContext = feedContext;

    public async Task CreateAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        await _feedContext.Comments.AddAsync(comment, cancellationToken: cancellationToken);
    }

    public Task DeleteAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        _feedContext.Comments.Remove(comment);

        return Task.CompletedTask;
    }

    public Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _feedContext.Comments.FirstOrDefaultAsync(r => r.Id == id, cancellationToken: cancellationToken);
    }
}