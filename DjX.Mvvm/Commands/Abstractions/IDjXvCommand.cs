namespace DjK.BackupTool.ViewModel.Commands.Abstractions;

public interface IDjXvCommand : IDjXvBaseCommand
{
    void Execute();
}

public interface IDjXvCommand<T> : IDjXvBaseCommand
{
    void Execute(T parameter);
}
