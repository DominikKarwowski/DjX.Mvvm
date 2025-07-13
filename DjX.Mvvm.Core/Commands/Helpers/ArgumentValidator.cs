namespace DjX.Mvvm.Core.Commands.Helpers;

public static class ArgumentValidator
{
    public static void ThrowIfNullOrNotOfType<T>(object? parameter)
    {
        if (parameter?.GetType() != typeof(T))
        {
            throw new ArgumentException("The parameter must be of type " + typeof(T).FullName);
        }
    }
}
