namespace ResultPattern.Api.Common;

/// <summary>
/// Represents the outcome of an operation that can either succeed (with a value)
/// or fail (with an Error). Inspired by functional programming Result/Either types.
/// </summary>
public sealed class Result<TValue>
{
    private readonly TValue? _value;

    private Result(TValue value)
    {
        _value = value;
        Error = Error.None;
        IsSuccess = true;
    }

    private Result(Error error)
    {
        _value = default;
        Error = error;
        IsSuccess = false;
    }

    /// <summary>Indicates whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Indicates whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>The error associated with a failed result.</summary>
    public Error Error { get; }

    /// <summary>
    /// Gets the value when the result is successful.
    /// Throws <see cref="InvalidOperationException"/> on failure.
    /// </summary>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value of a failed Result.");

    /// <summary>Creates a successful result containing <paramref name="value"/>.</summary>
    public static Result<TValue> Success(TValue value) => new(value);

    /// <summary>Creates a failed result containing <paramref name="error"/>.</summary>
    public static Result<TValue> Failure(Error error) => new(error);

    /// <summary>Implicit conversion from a value to a successful result.</summary>
    public static implicit operator Result<TValue>(TValue value) => Success(value);

    /// <summary>Implicit conversion from an error to a failed result.</summary>
    public static implicit operator Result<TValue>(Error error) => Failure(error);

    /// <summary>
    /// Executes <paramref name="onSuccess"/> when successful, otherwise <paramref name="onFailure"/>.
    /// </summary>
    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess(_value!) : onFailure(Error);
}

/// <summary>
/// Non-generic result for operations that produce no value on success.
/// </summary>
public sealed class Result
{
    private Result()
    {
        Error = Error.None;
        IsSuccess = true;
    }

    private Result(Error error)
    {
        Error = error;
        IsSuccess = false;
    }

    /// <summary>Indicates whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Indicates whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>The error associated with a failed result.</summary>
    public Error Error { get; }

    /// <summary>A cached successful result instance.</summary>
    public static readonly Result Ok = new();

    /// <summary>Creates a failed result containing <paramref name="error"/>.</summary>
    public static Result Failure(Error error) => new(error);

    /// <summary>Implicit conversion from an error to a failed result.</summary>
    public static implicit operator Result(Error error) => Failure(error);

    /// <summary>
    /// Executes <paramref name="onSuccess"/> when successful, otherwise <paramref name="onFailure"/>.
    /// </summary>
    public TResult Match<TResult>(Func<TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error);
}
