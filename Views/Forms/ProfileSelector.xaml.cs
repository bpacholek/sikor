using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sikor.Views.Forms
{
    public class ProfileSelector : UserControl
    {
        public ProfileSelector()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
