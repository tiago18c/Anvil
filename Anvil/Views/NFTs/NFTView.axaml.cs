using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Anvil.Views.NFTs
{
    public partial class NFTView : UserControl
    {
        public NFTView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
