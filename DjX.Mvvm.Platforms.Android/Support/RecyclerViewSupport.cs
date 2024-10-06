using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using DjX.Mvvm.Core.ViewModels;
using DjX.Mvvm.Platforms.Android.Binding;
using DjX.Mvvm.Platforms.Android.Resources;
using System.Collections.ObjectModel;
using System.Xml;

namespace DjX.Mvvm.Platforms.Android.Support;

public class BindableRecyclerViewAdapter<TCollectionDataType> : RecyclerView.Adapter
    where TCollectionDataType : ViewModelBase
{
    private readonly int itemTemplateLayoutId;
    private readonly string? itemBindingDeclaration;
    private readonly Dictionary<int, string> elementBindingDeclarations = [];
    private readonly ViewModelBase ParentViewModel;
    private readonly ObservableCollection<TCollectionDataType> DataSet;

    public BindableRecyclerViewAdapter(
        ObservableCollection<TCollectionDataType> dataSet,
        ViewModelBase parentViewModel,
        Context context,
        int itemTemplateLayoutId,
        string? itemBindingDeclaration)
    {
        this.itemTemplateLayoutId = itemTemplateLayoutId;
        this.itemBindingDeclaration = itemBindingDeclaration;
        this.ParentViewModel = parentViewModel;
        this.DataSet = dataSet;

        this.SetElementBindingData(context);
    }

    public class BindableRecycleViewHolder : RecyclerView.ViewHolder
    {
        internal View View { get; private set; }
        internal BindingObject BindingObject { get; } = new();

        public BindableRecycleViewHolder(View view)
            : base(view)
            => this.View = view;
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var view = LayoutInflater.From(parent.Context)!
            .Inflate(
                this.itemTemplateLayoutId,
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

        if (this.itemBindingDeclaration is not null)
        {
            bindableHolder.BindingObject.RegisterCollectionItemDeclaredEventBindings(
                bindableHolder.View, this.ParentViewModel, this.itemBindingDeclaration, this.DataSet[position]);
        }

        foreach (var kvp in this.elementBindingDeclarations)
        {
            var view = bindableHolder.View.FindViewById(kvp.Key)!;
            bindableHolder.BindingObject.RegisterDeclaredBindings(view, this.DataSet[position], kvp.Value);
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
        using var viewXml = context.Resources?.GetXml(this.itemTemplateLayoutId);

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
                this.elementBindingDeclarations.Add(int.Parse(idAttr[1..]), bindAttr);
            }
        }
    }
}
