using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sikor.Views
{
    public class FullLoader : UserControl
    {
        public FullLoader()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
