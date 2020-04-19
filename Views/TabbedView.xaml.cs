using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sikor.Views
{
    public class TabbedView : UserControl
    {
        public TabbedView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
