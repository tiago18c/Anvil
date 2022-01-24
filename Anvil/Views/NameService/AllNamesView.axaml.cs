using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Anvil.Views.NameService
{
    public partial class AllNamesView : UserControl
    {
        public AllNamesView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
