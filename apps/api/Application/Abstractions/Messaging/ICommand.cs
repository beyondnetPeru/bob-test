using Application.Common.Models;
using MediatR;

namespace Application.Abstractions.Messaging;

/// <summary>
/// Marker for commands that mutate state and return a typed result wrapped in Result&lt;TResult&gt;.
/// </summary>
public interface ICommand<TResult> : IRequest<Result<TResult>>;

/// <summary>
/// Marker for commands that mutate state and return a void Result (update / delete).
/// </summary>
public interface ICommand : IRequest<Result>;

/// <summary>
/// Marker for queries that read state and return a typed result wrapped in Result&lt;TResult&gt;.
/// </summary>
public interface IQuery<TResult> : IRequest<Result<TResult>>;
