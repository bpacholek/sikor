using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sikor.Views.Windows
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

        }
    }
}
