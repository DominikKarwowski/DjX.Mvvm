using System.Windows.Input;

namespace DjX.Mvvm.Core.Commands.Abstractions;

public interface IDjXCommandBase : ICommand
{
    void RaiseCanExecuteChanged();
}
