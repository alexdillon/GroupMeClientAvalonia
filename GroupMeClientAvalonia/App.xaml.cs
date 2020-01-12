using System.Windows.Input;
using Avalonia;
using Avalonia.Markup.Xaml;
using GalaSoft.MvvmLight.Command;

namespace GroupMeClientAvalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
