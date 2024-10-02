using AndroidX.RecyclerView.Widget;
using System.Collections.Specialized;

namespace DjX.Mvvm.Platforms.Android.Binding;

public class RecyclerViewCollectionBindingSet : IDisposable
{
    protected readonly INotifyCollectionChanged SourceCollection;
    protected readonly RecyclerView.Adapter TargetAdapter;

    private bool disposedValue;

    public RecyclerViewCollectionBindingSet(
        INotifyCollectionChanged sourceCollection,
        RecyclerView.Adapter targetAdapter)
    {
        this.SourceCollection = sourceCollection;
        this.TargetAdapter = targetAdapter;

        this.SourceCollection.CollectionChanged += this.OnObservableCollectionChanged;
    }

    private void OnObservableCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Action x = e.Action switch
        {
            NotifyCollectionChangedAction.Add => () => this.TargetAdapter.NotifyItemInserted(e.NewStartingIndex),
            NotifyCollectionChangedAction.Remove => () => this.TargetAdapter.NotifyItemRemoved(e.OldStartingIndex),
            NotifyCollectionChangedAction.Replace => () => this.TargetAdapter.NotifyItemChanged(e.OldStartingIndex),
            NotifyCollectionChangedAction.Move => () => this.TargetAdapter.NotifyItemMoved(e.OldStartingIndex, e.NewStartingIndex),
            NotifyCollectionChangedAction.Reset => () => this.TargetAdapter.NotifyDataSetChanged(),
            _ => () => this.TargetAdapter.NotifyDataSetChanged(),
        };

        x.Invoke();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.SourceCollection.CollectionChanged -= this.OnObservableCollectionChanged;
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
