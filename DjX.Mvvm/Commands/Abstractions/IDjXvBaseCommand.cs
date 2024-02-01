using System.Windows.Input;

namespace DjX.Mvvm.Commands.Abstractions;

public interface IDjXvBaseCommand : ICommand
{
    void RaiseCanExecuteChanged();
}
