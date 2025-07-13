namespace DjX.Mvvm.Core.Commands.Helpers;

public static class ArgumentValidator
{
    public static void ThrowIfNotOfType<T>(object? parameter)
    {
        if (parameter is not null && parameter.GetType() != typeof(T))
        {
            throw new ArgumentException("Non null parameter must be of type " + typeof(T).FullName);
        }
    }
}
