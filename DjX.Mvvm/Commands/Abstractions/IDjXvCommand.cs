namespace DjX.Mvvm.Commands.Abstractions;

public interface IDjXvCommand : IDjXvBaseCommand
{
    void Execute();
}

public interface IDjXvCommand<T> : IDjXvBaseCommand
{
    void Execute(T parameter);
}
