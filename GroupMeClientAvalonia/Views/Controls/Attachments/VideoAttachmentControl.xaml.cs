﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GroupMeClientAvalonia.Views.Controls.Attachments
{
    public class VideoAttachmentControl : UserControl
    {
        public VideoAttachmentControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}