using Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Application.Abstractions.Controllers;

/// <summary>
/// Base controller providing the MediatR mediator and common Result-to-HTTP mapping helpers.
/// All feature controllers must inherit from this class.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase(IMediator mediator) : ControllerBase
{
    protected readonly IMediator Mediator = mediator;

    // ──────────────────────────────────────────────────────────────────────
    // Query helpers
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Maps a <see cref="Result{T}"/> to 200 OK or an appropriate error response.
    /// Use for GET by-list and GET by-id actions.
    /// </summary>
    protected ActionResult<T> HandleQuery<T>(Result<T> result)
        => result.IsSuccess
            ? Ok(result.Value)
            : MapError<T>(result.Error);

    // ──────────────────────────────────────────────────────────────────────
    // Command helpers
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Maps a <see cref="Result{Guid}"/> to 201 Created or an appropriate error response.
    /// Use for POST (create) actions.
    /// </summary>
    protected ActionResult<Guid> HandleCreate(Result<Guid> result, string getActionName)
        => result.Match<ActionResult<Guid>>(
            id => CreatedAtAction(getActionName, new { id }, id),
            error => MapError<Guid>(error));

    /// <summary>
    /// Maps a void <see cref="Result"/> to 204 No Content or an appropriate error response.
    /// Use for PUT (update) and DELETE actions.
    /// </summary>
    protected IActionResult HandleCommand(Result result)
        => result.IsSuccess
            ? NoContent()
            : MapError(result.Error);

    /// <summary>
    /// Guards against route ID / body ID mismatch before dispatching a PUT command.
    /// Returns a <see cref="BadRequestObjectResult"/> if the IDs do not match, otherwise null.
    /// </summary>
    protected BadRequestObjectResult? ValidateIdMatch(Guid routeId, Guid bodyId)
        => routeId != bodyId
            ? BadRequest(new { Code = "Error.IdMismatch", Message = $"Route ID '{routeId}' does not match body ID '{bodyId}'." })
            : null;

    // ──────────────────────────────────────────────────────────────────────
    // Error mapping
    // ──────────────────────────────────────────────────────────────────────

    private ActionResult MapError(Error error)
        => error.Code switch
        {
            Error.NotFoundCode => NotFound(ToProblem(404, "Not Found", error.Message)),
            Error.ValidationCode => BadRequest(ToProblem(400, "Validation Error", error.Message)),
            Error.DomainCode => UnprocessableEntity(ToProblem(422, "Domain Error", error.Message)),
            _ => BadRequest(ToProblem(400, "Bad Request", error.Message))
        };

    private ActionResult<T> MapError<T>(Error error)
        => error.Code switch
        {
            Error.NotFoundCode => NotFound(ToProblem(404, "Not Found", error.Message)),
            Error.ValidationCode => BadRequest(ToProblem(400, "Validation Error", error.Message)),
            Error.DomainCode => UnprocessableEntity(ToProblem(422, "Domain Error", error.Message)),
            _ => BadRequest(ToProblem(400, "Bad Request", error.Message))
        };

    private static ProblemDetails ToProblem(int status, string title, string detail)
        => new() { Status = status, Title = title, Detail = detail };
}
