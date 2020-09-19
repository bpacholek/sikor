using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sikor.Views.Forms
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
