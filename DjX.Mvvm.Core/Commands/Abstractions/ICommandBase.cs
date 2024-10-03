using System.Windows.Input;

namespace DjX.Mvvm.Core.Commands.Abstractions;

public interface ICommandBase : ICommand
{
    void RaiseCanExecuteChanged();
}
