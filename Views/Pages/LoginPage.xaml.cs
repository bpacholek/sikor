using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sikor.Views.Pages
{
    public class LoginPage : UserControl
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
