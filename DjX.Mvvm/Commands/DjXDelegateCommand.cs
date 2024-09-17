using DjX.Mvvm.Commands.Abstractions;

namespace DjX.Mvvm.Commands;


public class DjXDelegateCommand<T> : IDjXCommand<T>
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public DjXDelegateCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        this._execute = execute;
        this._canExecute = canExecute;
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => this._canExecute is null || this._canExecute((T?)parameter);
    public void Execute(object? parameter) => this._execute((T?)parameter);
    public void Execute(T parameter) => this._execute(parameter);
}


public class DjXDelegateCommand : IDjXCommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public DjXDelegateCommand(Action execute, Func<bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        this._execute = new Action<object?>(param => execute());
        this._canExecute = canExecute is null
            ? null
            : new Func<object?, bool>(param => canExecute());
    }

    public DjXDelegateCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        this._execute = execute;
        this._canExecute = canExecute;
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => this._canExecute is null || this._canExecute(parameter);
    public void Execute(object? parameter) => this._execute(parameter);
    public void Execute() => this.Execute(null);

}
