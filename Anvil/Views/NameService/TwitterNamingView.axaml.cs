using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Anvil.Views.NameService
{
    public partial class TwitterNamingView : UserControl
    {
        public TwitterNamingView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
