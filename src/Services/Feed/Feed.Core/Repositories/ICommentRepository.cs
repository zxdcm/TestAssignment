using Feed.Core.Entities;

namespace Feed.Core.Repositories;

public interface ICommentRepository : IBaseRepository
{
    Task CreateAsync(Comment comment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Comment comment, CancellationToken cancellationToken = default);
    Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}