using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sikor.Views.Utils
{
    public class OnTopLoader : UserControl
    {
        public OnTopLoader()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
