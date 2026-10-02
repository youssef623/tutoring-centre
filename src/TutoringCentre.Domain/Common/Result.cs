using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Domain.Common;

/// <summary>The outcome of an operation that can fail for an expected business reason.</summary>
public class Result
{
    protected Result(bool isSuccess, Error? error)
    {
        // The only two legal states: success without an error, failure with one.
        if (isSuccess && error is not null)
        {
            throw new ArgumentException("A successful result cannot carry an error.", nameof(error));
        }

        if (!isSuccess && error is null)
        {
            throw new ArgumentNullException(nameof(error), "A failed result must carry an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    /// <summary>The failure's error; null when <see cref="IsSuccess"/> is true.</summary>
    public Error? Error { get; }

    public static Result Success() => new(isSuccess: true, error: null);

    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static Result Failure(Error error) => new(isSuccess: false, error);
}

/// <summary>The outcome of an operation that produces a <typeparamref name="T"/> or fails for an expected business reason.</summary>
[SuppressMessage(
    "Design",
    "CA1000:Do not declare static members on generic types",
    Justification = "Result<T>.Success/Failure are the agreed factory API; call sites always name the type.")]
public sealed class Result<T> : Result
{
    private readonly T _value;

    private Result(T value)
        : base(isSuccess: true, error: null)
    {
        _value = value;
    }

    private Result(Error error)
        : base(isSuccess: false, error)
    {
        _value = default!;
    }

    /// <summary>The produced value.</summary>
    /// <exception cref="InvalidOperationException">The result is a failure — reading it is a bug.</exception>
    public T Value => IsSuccess
        ? _value
        : throw new InvalidOperationException(
            $"Cannot read Value of a failed result (error code '{Error?.Code}'). Check IsSuccess first.");

    public static Result<T> Success(T value) => new(value);

    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static new Result<T> Failure(Error error) => new(error);
}
