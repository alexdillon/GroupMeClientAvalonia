using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace GroupMeClientAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public ChatsViewModel ChatsView { get; set; } = new ChatsViewModel();
    }
}
