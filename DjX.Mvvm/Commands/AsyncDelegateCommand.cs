using DjK.BackupTool.ViewModel.Commands.Abstractions;

namespace DjK.BackupTool.ViewModel.Commands;


//https://learn.microsoft.com/en-us/archive/msdn-magazine/2014/april/async-programming-patterns-for-asynchronous-mvvm-applications-commands
//https://johnthiriet.com/mvvm-going-async-with-async-command/

public class AsyncDelegateCommand<T> : IDjXvAsyncCommand<T>, IDisposable
{
    private readonly Func<T?, Task> _execute;
    private readonly Func<T?, bool>? _canExecute;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool disposedValue;

    public AsyncDelegateCommand(Func<T?, Task> execute, Func<T?, bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        _execute = execute;
        _canExecute = canExecute;
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute is null || _canExecute((T?)parameter);
    public async void Execute(object? parameter) => await ExecuteAsync((T?)parameter);

    public async Task ExecuteAsync(T? parameter)
    {
        await _semaphore.WaitAsync();
        try
        {
            await _execute(parameter);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _semaphore.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class AsyncDelegateCommand : IDjXvAsyncCommand, IDisposable
{
    private readonly Func<object?, Task> _execute;
    private readonly Func<object?, bool>? _canExecute;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool disposedValue;

    public AsyncDelegateCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        _execute = new Func<object?, Task>(param => execute());
        _canExecute = canExecute is null
            ? null
            : new Func<object?, bool>(param => canExecute());
    }

    public AsyncDelegateCommand(Func<object?, Task> execute, Func<object?, bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        _execute = execute;
        _canExecute = canExecute;
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute is null || _canExecute(parameter);
    public async void Execute(object? parameter) => await ExecuteAsync();

    public async Task ExecuteAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            await _execute(null);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _semaphore.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
