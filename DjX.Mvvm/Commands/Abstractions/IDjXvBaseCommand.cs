using System.Windows.Input;

namespace DjK.BackupTool.ViewModel.Commands.Abstractions;

public interface IDjXvBaseCommand : ICommand
{
    void RaiseCanExecuteChanged();
}
