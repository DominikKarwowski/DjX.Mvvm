using DjX.Mvvm.Core.Commands.Abstractions;

namespace DjX.Mvvm.Core.Commands;

//https://learn.microsoft.com/en-us/archive/msdn-magazine/2014/april/async-programming-patterns-for-asynchronous-mvvm-applications-commands
//https://johnthiriet.com/mvvm-going-async-with-async-command/

public class DjXAsyncDelegateCommand<T> : IDjXCommandBase, IDisposable
{
    private readonly Func<T?, Task> _execute;
    private readonly Func<T?, bool>? _canExecute;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool disposedValue;

    public DjXAsyncDelegateCommand(Func<T?, Task> execute, Func<T?, bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        this._execute = execute;
        this._canExecute = canExecute;
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => this._canExecute is null || this._canExecute((T?)parameter);
    public async void Execute(object? parameter) => await this.ExecuteAsync((T?)parameter);

    public async Task ExecuteAsync(T? parameter)
    {
        await this._semaphore.WaitAsync();
        try
        {
            await this._execute(parameter);
        }
        finally
        {
            _ = this._semaphore.Release();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this._semaphore.Dispose();
            }

            this.disposedValue = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class DjXAsyncDelegateCommand : IDjXCommandBase, IDisposable
{
    private readonly Func<object?, Task> _execute;
    private readonly Func<object?, bool>? _canExecute;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool disposedValue;

    public DjXAsyncDelegateCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        this._execute = new Func<object?, Task>(param => execute());
        this._canExecute = canExecute is null
            ? null
            : new Func<object?, bool>(param => canExecute());
    }

    public DjXAsyncDelegateCommand(Func<object?, Task> execute, Func<object?, bool>? canExecute = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        this._execute = execute;
        this._canExecute = canExecute;
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => this._canExecute is null || this._canExecute(parameter);
    public async void Execute(object? parameter) => await this.ExecuteAsync();

    public async Task ExecuteAsync()
    {
        await this._semaphore.WaitAsync();
        try
        {
            await this._execute(null);
        }
        finally
        {
            _ = this._semaphore.Release();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this._semaphore.Dispose();
            }

            this.disposedValue = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
