namespace DjX.Mvvm.Core.Commands.Abstractions;

public interface IDjXAsyncCommand : IDjXBaseCommand
{
    Task ExecuteAsync();
}

public interface IDjXAsyncCommand<T> : IDjXBaseCommand
{
    Task ExecuteAsync(T? parameter);
}
