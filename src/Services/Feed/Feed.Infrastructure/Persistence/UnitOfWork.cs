using Feed.Core;
using Feed.Infrastructure.Persistence.ModelBuilding;

namespace Feed.Infrastructure.Persistence;

public class UnitOfWork(FeedContext feedContext) : IUnitOfWork
{
    public async Task CommitAsync()
    {
        await feedContext.SaveChangesAsync();
    }
}