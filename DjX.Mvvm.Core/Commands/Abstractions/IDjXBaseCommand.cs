using System.Windows.Input;

namespace DjX.Mvvm.Core.Commands.Abstractions;

public interface IDjXBaseCommand : ICommand
{
    void RaiseCanExecuteChanged();
}
