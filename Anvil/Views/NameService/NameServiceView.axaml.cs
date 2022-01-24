using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Anvil.Views.NameService
{
    public partial class NameServiceView : UserControl
    {
        public NameServiceView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
