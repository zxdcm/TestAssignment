using MediatR;

namespace Feed.Core.Cqrs.Query;

public interface IQuery<out TResponse> : IRequest<TResponse>
{ }
