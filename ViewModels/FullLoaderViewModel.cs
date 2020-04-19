using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using Sikor.Services;
using Sikor.Container;

namespace Sikor.ViewModels
{
    public class FullLoaderViewModel : ReactiveViewServiceProvider
    {
        protected bool loaderVisible = false;

        public bool LoaderVisible
        {
            get => loaderVisible;
            set => this.RaiseAndSetIfChanged(ref loaderVisible, value);
        }

        public void Show()
        {
            LoaderVisible = true;
        }

        public void Hide()
        {
            LoaderVisible = false;
        }

    }
}
