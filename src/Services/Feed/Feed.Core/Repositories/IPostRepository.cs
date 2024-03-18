using Feed.Core.Entities;

namespace Feed.Core.Repositories;

public interface IPostRepository : IBaseRepository
{
    Task CreateAsync(Post post, CancellationToken cancellationToken = default);
}