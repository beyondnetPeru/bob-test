namespace Application.Common.Models;

public sealed record Error(string Code, string Message)
{
    // Shared constants — used by ResultExtensions for HTTP mapping
    public const string NotFoundCode = "Error.NotFound";
    public const string ValidationCode = "Error.Validation";
    public const string DomainCode = "Error.Domain";
    public const string NullValueCode = "Error.NullValue";

    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new(NullValueCode, "The specified result value is null.");

    public static Error Validation(string message) => new(ValidationCode, message);
    public static Error NotFound(string name, object key) => new(NotFoundCode, $"Entity '{name}' ({key}) was not found.");
    public static Error Domain(string message) => new(DomainCode, message);

    public static implicit operator string(Error error) => error.Code;
}
