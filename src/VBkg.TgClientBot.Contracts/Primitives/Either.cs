using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace VBkg.TgClientBot;

[DebuggerDisplay("{DebuggerDisplay()}")]
public readonly struct Either<TL, TR>
{
    private readonly TL _left;
    private readonly TR _right;
    private readonly bool _isLeft;

    public Either(TL left)
    {
        _isLeft = true;
        _left = left;
        _right = default!;
    }

    public Either(TR right)
    {
        _isLeft = false;
        _left = default!;
        _right = right;
    }

    public T Match<T>(Func<TL, T> leftFunc, Func<TR, T> rightFunc)
    {
        if (_isLeft)
        {
            return leftFunc(_left);
        }

        return rightFunc(_right);
    }

    public async Task<T> MatchAsync<T>(Func<TL, ValueTask<T>> leftFunc, Func<TR, ValueTask<T>> rightFunc)
    {
        if (_isLeft)
        {
            return await leftFunc(_left);
        }

        return await rightFunc(_right);
    }

    public void Execute(Action<TL> leftAction,
        Action<TR> rightAction)
    {
        if (_isLeft)
        {
            leftAction(_left);
            return;
        }

        rightAction(_right);
    }

    public async ValueTask ExecuteAsync(Func<TL, ValueTask> leftAction,
        Func<TR, ValueTask> rightAction)
    {
        if (_isLeft)
        {
            await leftAction(_left);
            return;
        }

        await rightAction(_right);
    }

    public void Execute<THelper>(THelper helper,
        Action<TL, THelper> leftAction,
        Action<TR, THelper> rightAction)
    {
        if (_isLeft)
        {
            leftAction(_left, helper);
            return;
        }

        rightAction(_right, helper);
    }

    public TL LeftOrDefault => _left!;
    public TR RightOrDefault => _right!;

    public static implicit operator Either<TL, TR>(TL left) => new Either<TL, TR>(left);
    public static implicit operator Either<TL, TR>(TR right) => new Either<TL, TR>(right);

    private string? DebuggerDisplay()
    {
        if (_right is not null)
            return _right.ToString();

        if (_left is not null)
            return _left.ToString();

        return "[INVALID_STATE]";
    }
}