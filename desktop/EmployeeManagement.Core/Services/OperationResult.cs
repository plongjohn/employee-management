namespace EmployeeManagement.Core.Services;

/// <summary>
/// Outcome of a write operation. Expected failures (validation, duplicate email, conflicts)
/// are results, not exceptions; exceptions are reserved for unexpected errors.
/// </summary>
public class OperationResult
{
    protected OperationResult(OperationStatus status, IReadOnlyList<ValidationError> errors)
    {
        Status = status;
        Errors = errors;
    }

    public OperationStatus Status { get; }
    public IReadOnlyList<ValidationError> Errors { get; }
    public bool IsSuccess => Status == OperationStatus.Success;

    public static OperationResult Success() => new(OperationStatus.Success, []);
    public static OperationResult Failure(OperationStatus status) => new(EnsureFailure(status), []);

    protected static OperationStatus EnsureFailure(OperationStatus status) =>
        status is OperationStatus.Success or OperationStatus.ValidationFailed
            ? throw new ArgumentException(
                "Use Success() or ValidationFailed() for this status.", nameof(status))
            : status;

    protected static IReadOnlyList<ValidationError> EnsureErrors(IReadOnlyList<ValidationError> errors) =>
        errors.Count == 0
            ? throw new ArgumentException("A validation failure needs at least one error.", nameof(errors))
            : errors;
}

public sealed class OperationResult<T> : OperationResult
{
    private OperationResult(OperationStatus status, IReadOnlyList<ValidationError> errors, T? value)
        : base(status, errors)
    {
        Value = value;
    }

    /// <summary>Set only when <see cref="OperationResult.IsSuccess"/> is true.</summary>
    public T? Value { get; }

    public static OperationResult<T> Success(T value) => new(OperationStatus.Success, [], value);

    public static OperationResult<T> ValidationFailed(IReadOnlyList<ValidationError> errors) =>
        new(OperationStatus.ValidationFailed, EnsureErrors(errors), default);

    public static new OperationResult<T> Failure(OperationStatus status) =>
        new(EnsureFailure(status), [], default);
}
