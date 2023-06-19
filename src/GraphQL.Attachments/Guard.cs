static class Guard
{
    public static void AgainstNullWhiteSpace(string value, [CallerArgumentExpression("value")]  string argumentName = "")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static Func<T> WrapFuncInCheck<T>(this Func<T> func, string name) =>
        () => func.EvaluateAndCheck(name);

    static T EvaluateAndCheck<T>(this Func<T> func, string attachment)
    {
        var message = $"Provided delegate threw an exception. Attachment: {attachment}.";
        T value;
        try
        {
            value = func();
        }
        catch (Exception exception)
        {
            throw new(message, exception);
        }

        ThrowIfNullReturned(null, attachment, value);
        return value;
    }

    static async Task<T> EvaluateAndCheck<T>(
        this Func<Cancel, Task<T>> func,
        string attachment,
        Cancel cancel)
    {
        var message = $"Provided delegate threw an exception. Attachment: {attachment}.";
        T value;
        try
        {
            value = await func(cancel);
        }
        catch (Exception exception)
        {
            throw new(message, exception);
        }

        ThrowIfNullReturned(null, attachment, value);
        return value;
    }

    public static Action? WrapCleanupInCheck(this Action? cleanup, string attachment)
    {
        if (cleanup == null)
        {
            return null;
        }

        return () =>
        {
            try
            {
                cleanup();
            }
            catch (Exception exception)
            {
                throw new($"Cleanup threw an exception. Attachment: {attachment}.", exception);
            }
        };
    }

    public static Func<Cancel, Task<T>> WrapFuncTaskInCheck<T>(
        this Func<Cancel, Task<T>> func,
        string attachment) =>
        async cancel =>
        {
            var task = func.EvaluateAndCheck(attachment, cancel);
            ThrowIfNullReturned(null, attachment, task);
            var value = await task;
            ThrowIfNullReturned(null, attachment, value);
            return value;
        };

    public static Func<Cancel, Task<Stream>> WrapStreamFuncTaskInCheck<T>(
        this Func<Cancel, Task<T>> func,
        string attachment)
        where T : Stream =>
        async cancel =>
        {
            var task = func.EvaluateAndCheck(attachment, cancel);
            ThrowIfNullReturned(null, attachment, task);
            var value = await task;
            ThrowIfNullReturned(null, attachment, value);
            return value;
        };

    public static void ThrowIfNullReturned(string? messageId, string attachment, object? value)
    {
        if (value != null)
        {
            return;
        }

        if (attachment != null && messageId != null)
        {
            throw new($"Provided delegate returned a null. MessageId: '{messageId}', Attachment: '{attachment}'.");
        }

        if (attachment != null)
        {
            throw new($"Provided delegate returned a null. Attachment: '{attachment}'.");
        }

        if (messageId != null)
        {
            throw new($"Provided delegate returned a null. MessageId: '{messageId}'.");
        }

        throw new("Provided delegate returned a null.");
    }
}