using System.Windows.Input;

namespace DjX.Mvvm.Commands.Abstractions;

public interface IDjXBaseCommand : ICommand
{
    void RaiseCanExecuteChanged();
}
