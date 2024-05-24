using Application.Shared.Response;

namespace Application.Shared.Responses;

/// <summary>
/// Represents the result of some operation, with status information and possibly a value and an error.
/// </summary>
/// <typeparam name="TValue">The result value type.</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;
    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error) =>_value = value;
    public TValue Value => IsSuccess
        ? _value!: throw new InvalidOperationException("The value of a failure result can not be accessed.");
   
    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}
