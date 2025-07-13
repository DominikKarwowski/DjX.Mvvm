using DjX.Mvvm.Core.Commands.Helpers;

namespace DjX.Mvvm.Core.Commands;

using System.Windows.Input;
using DjX.Mvvm.Core.Commands.Abstractions;

public class DelegateCommand<T> : ICommandBase
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public DelegateCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute, nameof(execute));
        this._execute = execute;
        this._canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    public bool CanExecute(T? parameter) => this._canExecute is null || this._canExecute(parameter);
    public void Execute(T? parameter) => this._execute(parameter);
    
    bool ICommand.CanExecute(object? parameter)
    {
        ArgumentValidator.ThrowIfNotOfType<T?>(parameter);
        
        return this.CanExecute((T?)parameter);
    }
    
    void ICommand.Execute(object? parameter)
    {
        ArgumentValidator.ThrowIfNotOfType<T?>(parameter);
        
        this.Execute((T?)parameter);
    }
}


public class DelegateCommand : ICommandBase
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public DelegateCommand(Action execute, Func<bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute, nameof(execute));
        this._execute = execute;
        this._canExecute = canExecute;
    }
    
    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    public bool CanExecute() => this._canExecute is null || this._canExecute();
    public void Execute() => this._execute();
    bool ICommand.CanExecute(object? parameter) => this.CanExecute();
    void ICommand.Execute(object? parameter) => this.Execute();
}
