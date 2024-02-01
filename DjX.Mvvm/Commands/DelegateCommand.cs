using DjX.Mvvm.Commands.Abstractions;

namespace DjX.Mvvm.Commands;


public class DelegateCommand<T> : IDjXvCommand<T>
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public DelegateCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        _execute = execute;
        _canExecute = canExecute;
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute is null || _canExecute((T?)parameter);
    public void Execute(object? parameter) => _execute((T?)parameter);
    public void Execute(T parameter) => _execute(parameter);
}


public class DelegateCommand : IDjXvCommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public DelegateCommand(Action execute, Func<bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        _execute = new Action<object?>(param => execute());
        _canExecute = canExecute is null
            ? null
            : new Func<object?, bool>(param => canExecute());
    }

    public DelegateCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        _execute = execute;
        _canExecute = canExecute;
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute is null || _canExecute(parameter);
    public void Execute(object? parameter) => _execute(parameter);
    public void Execute() => Execute(null);

}
