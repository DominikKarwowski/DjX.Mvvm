namespace DjX.Mvvm.Commands.Abstractions;

public interface IDjXvAsyncCommand : IDjXvBaseCommand
{
    Task ExecuteAsync();
}

public interface IDjXvAsyncCommand<T> : IDjXvBaseCommand
{
    Task ExecuteAsync(T? parameter);
}
