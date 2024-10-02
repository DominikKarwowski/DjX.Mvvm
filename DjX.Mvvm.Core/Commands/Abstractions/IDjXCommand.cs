namespace DjX.Mvvm.Core.Commands.Abstractions;

public interface IDjXCommand : IDjXBaseCommand
{
    void Execute();
}

public interface IDjXCommand<T> : IDjXBaseCommand
{
    void Execute(T parameter);
}
