using System.Reflection;

namespace DjX.Mvvm.Core.ViewModels.Factories;

// TODO: resolve viewmodel dependencies with a dedicated resolver in the composition root:
// * resolver will inspect available viewmodel ctors and look for available registered dependencies
// * viewmodelfactory will take resolved dependencies as an array of objects to be passed to Activator.Createinstance
// * create a dedicated extension method to register viewmodelfactory and apply params resolver
// Once this is done, remove Microsoft.Extensions.DependencyInjection package dependency from this project

// TODO: refactor the below hastily coded poc into a better desing
public class ViewModelFactory<TViewModel>(IServiceProvider serviceProvider)
    where TViewModel : ViewModelBase
{
    public TViewModel CreateViewModel()
    {
        var viewModelType = typeof(TViewModel);

        var viewModelCtors = viewModelType
            .GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length); // TODO: memoize ctor params and pass it to resolver method ?

        var viewModelCtorArgs = this.ResolveConstructorArgs(viewModelCtors);

        return Activator.CreateInstance(viewModelType, viewModelCtorArgs) as TViewModel
            // TODO: throw more specific exception
            ?? throw new Exception("Could not create a view model");
    }

    private object[] ResolveConstructorArgs(IEnumerable<ConstructorInfo> viewModelCtors)
    {
        foreach (var ctor in viewModelCtors)
        {
            var ctorParamsResolved = true;

            var ctorParams = ctor.GetParameters();
            var ctorArgs = new List<object>();

            foreach (var param in ctorParams)
            {
                var instance = serviceProvider.GetService(param.ParameterType);

                if (instance is null)
                {
                    ctorParamsResolved = false;
                    break;
                }

                ctorArgs.Add(instance);
            }

            if (ctorParamsResolved)
            {
                return [.. ctorArgs];
            }
        }

        // TODO: properly choose more specific exception
        throw new Exception("Could not resolve constructor dependencies");
    }

    //public TViewModel CreateViewModel<TModel>(TModel model)
    public TViewModel CreateViewModel(object? model, Type modelType)
    {
        //var modelType = typeof(TModel);

        var viewModelType = typeof(TViewModel); //.MakeGenericType(modelType);

        var viewModelCtors = viewModelType
            .GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length);

        // This should be handled by the upper method
        // TODO: but what in case of constrained types in F#?
        //if (model is null)
        //{
        //    var newModel = (TModel)Activator.CreateInstance(modelType)!;

        //    return (TViewModel)Activator.CreateInstance(viewModelType, [newModel])!;
        //}
        var viewModelCtorArgs = this.ResolveConstructorArgs(viewModelCtors, model, modelType);

        return Activator.CreateInstance(viewModelType, viewModelCtorArgs) as TViewModel
            // TODO: throw more specific exception
            ?? throw new Exception("Could not create a view model");
    }

    private object[] ResolveConstructorArgs(IEnumerable<ConstructorInfo> viewModelCtors, object? model, Type modelType)
    {
        foreach (var ctor in viewModelCtors)
        {
            var ctorParamsResolved = true;

            var ctorParams = ctor.GetParameters();
            var ctorArgs = new List<object>();

            foreach (var param in ctorParams)
            {
                if (param.ParameterType == modelType)
                {
                    ctorArgs.Add(model!);
                    continue;
                }

                var instance = serviceProvider.GetService(param.ParameterType);

                if (instance is null)
                {
                    ctorParamsResolved = false;
                    break;
                }

                ctorArgs.Add(instance);
            }

            if (ctorParamsResolved)
            {
                return [.. ctorArgs];
            }
        }

        // TODO: properly choose more specific expection
        throw new Exception("Could not resolve constructor dependencies");
    }
}
