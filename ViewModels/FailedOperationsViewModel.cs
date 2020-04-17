using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using Sikor.Services;

namespace Sikor.ViewModels
{
    public class FailedOperationsViewModel : ReactiveObject, IService
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
        public FailedOperationsViewModel()
        {
        }
    }
}
