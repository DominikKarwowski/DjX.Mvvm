#if ANDROID21_0_OR_GREATER
using DjX.Mvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjX.Mvvm.Platforms.Android;
public abstract class DjXApplication : Application
{
    public abstract T CreateViewModel<T>()
        where T : ViewModelBase;
}
#endif