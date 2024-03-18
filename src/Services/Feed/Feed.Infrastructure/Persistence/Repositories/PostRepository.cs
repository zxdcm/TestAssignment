using Feed.Core.Entities;
using Feed.Core.Repositories;
using Feed.Infrastructure.Persistence.ModelBuilding;

namespace Feed.Infrastructure.Persistence.Repositories;

public class PostRepository(FeedContext feedContext) : IPostRepository
{
    private readonly FeedContext _feedContext = feedContext;

    public async Task CreateAsync(Post post, CancellationToken cancellationToken = default)
    {
        await _feedContext.Posts.AddAsync(post, cancellationToken);
    }
}