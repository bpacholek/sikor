using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sikor.Views
{
    public class ProfileCreator : UserControl
    {
        public ProfileCreator()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
