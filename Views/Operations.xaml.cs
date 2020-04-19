using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sikor.Views
{
    public class Operations : UserControl
    {
        public Operations()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
