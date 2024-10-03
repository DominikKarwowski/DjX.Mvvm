namespace DjX.Mvvm.Core.Commands.Abstractions;

public interface IDjXCommand : IDjXCommandBase
{
    void Execute();
}

public interface IDjXCommand<T> : IDjXCommandBase
{
    void Execute(T parameter);
}
