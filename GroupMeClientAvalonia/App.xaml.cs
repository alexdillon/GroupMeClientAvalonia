﻿using Avalonia;
using Avalonia.Markup.Xaml;

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