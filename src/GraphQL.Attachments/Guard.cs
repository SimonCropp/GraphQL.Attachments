using System;
using System.IO;
using System.Threading.Tasks;

static class Guard
{
    public static void AgainstNull(string argumentName, object value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstNullWhiteSpace(string argumentName, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstNegative(string argumentName, int value)
    {
        if (value < 0)
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static Func<T> WrapFuncInCheck<T>(this Func<T> func, string name)
    {
        return () => func.EvaluateAndCheck(name);
    }

    static T EvaluateAndCheck<T>(this Func<T> func, string attachmentName)
    {
        var message = $"Provided delegate threw an exception. Attachment name: {attachmentName}.";
        T value;
        try
        {
            value = func();
        }
        catch (Exception exception)
        {
            throw new Exception(message, exception);
        }

        ThrowIfNullReturned(null, attachmentName, value);
        return value;
    }

    public static Action? WrapCleanupInCheck(this Action? cleanup, string attachmentName)
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
                throw new Exception($"Cleanup threw an exception. Attachment name: {attachmentName}.", exception);
            }
        };
    }

    public static Func<Task<T>> WrapFuncTaskInCheck<T>(this Func<Task<T>> func, string attachmentName)
    {
        return async () =>
        {
            var task = func.EvaluateAndCheck(attachmentName);
            ThrowIfNullReturned(null, attachmentName, task);
            var value = await task;
            ThrowIfNullReturned(null, attachmentName, value);
            return value;
        };
    }

    public static Func<Task<Stream>> WrapStreamFuncTaskInCheck<T>(this Func<Task<T>> func, string attachmentName)
        where T : Stream
    {
        return async () =>
        {
            var task = func.EvaluateAndCheck(attachmentName);
            ThrowIfNullReturned(null, attachmentName, task);
            var value = await task;
            ThrowIfNullReturned(null, attachmentName, value);
            return value;
        };
    }

    public static void ThrowIfNullReturned(string? messageId, string attachmentName, object? value)
    {
        if (value != null)
        {
            return;
        }
        if (attachmentName != null && messageId != null)
        {
            throw new Exception($"Provided delegate returned a null. MessageId: '{messageId}', Attachment: '{attachmentName}'.");
        }

        if (attachmentName != null)
        {
            throw new Exception($"Provided delegate returned a null. Attachment: '{attachmentName}'.");
        }

        if (messageId != null)
        {
            throw new Exception($"Provided delegate returned a null. MessageId: '{messageId}'.");
        }

        throw new Exception("Provided delegate returned a null.");
    }
}