#if ANDROID21_0_OR_GREATER
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using DjX.Mvvm.Binding;
using DjX.Mvvm.Resources;
using DjX.Mvvm.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml;

namespace DjX.Mvvm.Platforms.Android;

public class BindableRecyclerViewAdapter<TCollectionDataType> : RecyclerView.Adapter
    where TCollectionDataType : ViewModelBase
{
    private readonly int _itemTemplateLayoutId;
    private readonly Dictionary<int, string> _elementBindingsToParse = [];

    public ObservableCollection<TCollectionDataType> DataSet { get; set; }

    public BindableRecyclerViewAdapter(ObservableCollection<TCollectionDataType> dataSet, Context context, int itemTemplateLayoutId)
    {
        this._itemTemplateLayoutId = itemTemplateLayoutId;
        this.DataSet = dataSet;

        this.SetElementBindingData(context);
    }

    public class BindableRecycleViewHolder : RecyclerView.ViewHolder
    {
        internal View View { get; private set; }
        internal AndroidBindingObject BindingObject { get; } = new();

        public BindableRecycleViewHolder(View view)
            : base(view)
            => this.View = view;
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var view = LayoutInflater.From(parent.Context)!
            .Inflate(
                this._itemTemplateLayoutId,
                parent,
                attachToRoot: false);

        return new BindableRecycleViewHolder(view!);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        if (holder is not BindableRecycleViewHolder bindableHolder)
        {
            return;
        }

        foreach (var kvp in this._elementBindingsToParse)
        {
            var view = bindableHolder.View.FindViewById(kvp.Key)!;
            bindableHolder.BindingObject.RegisterBindingSet(this.DataSet[position], view, kvp.Value);
        }
    }

    public override void OnViewRecycled(Java.Lang.Object holder)
    {
        if (holder is BindableRecycleViewHolder bindableHolder)
        {
            bindableHolder.BindingObject.Dispose();
        }

        base.OnViewRecycled(holder);
    }

    public override int ItemCount => this.DataSet.Count;

    private void SetElementBindingData(Context context)
    {
        using var viewXml = context.Resources?.GetXml(this._itemTemplateLayoutId);

        if (viewXml is null)
        {
            return;
        }

        _ = viewXml.MoveToContent();

        while (viewXml.Read())
        {
            if (viewXml.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            var idAttr = viewXml.GetAttribute(AndroidStrings.IdAttributeName, AndroidStrings.Namespace);
            var bindAttr = viewXml.GetAttribute(AndroidStrings.BindAttributeName, AndroidStrings.AppNamespace);

            if (idAttr is not null && bindAttr is not null)
            {
                this._elementBindingsToParse.Add(int.Parse(idAttr[1..]), bindAttr);
            }
        }
    }
}
#endif