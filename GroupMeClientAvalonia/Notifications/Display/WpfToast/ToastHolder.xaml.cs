using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GroupMeClientAvalonia.Notifications.Display.WpfToast
{
    public class ToastHolder : UserControl
    {
        public ToastHolder()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
