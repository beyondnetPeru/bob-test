using Application.Common.Models;
using MediatR;

namespace Application.Abstractions.Messaging;

/// <summary>
/// Typed handler contract for state-mutating commands that return a result value.
/// </summary>
public interface ICommandHandler<TCommand, TResult>
    : IRequestHandler<TCommand, Result<TResult>>
    where TCommand : ICommand<TResult>;

/// <summary>
/// Typed handler contract for fire-and-forget commands (update, delete).
/// </summary>
public interface ICommandHandler<TCommand>
    : IRequestHandler<TCommand, Result>
    where TCommand : ICommand;

/// <summary>
/// Typed handler contract for queries.
/// </summary>
public interface IQueryHandler<TQuery, TResult>
    : IRequestHandler<TQuery, Result<TResult>>
    where TQuery : IQuery<TResult>;
