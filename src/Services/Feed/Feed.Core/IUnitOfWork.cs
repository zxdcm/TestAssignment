namespace Feed.Core;

public interface IUnitOfWork
{
    public Task CommitAsync();
}