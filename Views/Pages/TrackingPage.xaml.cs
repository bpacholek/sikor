using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sikor.Views.Pages
{
    public class TrackingPage : UserControl
    {
        public TrackingPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
