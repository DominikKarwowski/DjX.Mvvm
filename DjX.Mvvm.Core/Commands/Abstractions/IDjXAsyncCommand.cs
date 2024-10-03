namespace DjX.Mvvm.Core.Commands.Abstractions;

public interface IDjXAsyncCommand : IDjXCommandBase
{
    Task ExecuteAsync();
}

public interface IDjXAsyncCommand<T> : IDjXCommandBase
{
    Task ExecuteAsync(T? parameter);
}
