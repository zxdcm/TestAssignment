using MediatR;

namespace Feed.Core.Cqrs.Query;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery: IQuery<TResponse>;