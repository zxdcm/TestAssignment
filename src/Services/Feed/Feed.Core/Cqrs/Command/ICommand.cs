using MediatR;

namespace Feed.Core.Cqrs.Command;

public interface ICommand<out TResponse> : IRequest<TResponse>;