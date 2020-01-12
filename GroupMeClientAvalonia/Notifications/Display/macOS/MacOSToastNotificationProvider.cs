using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AppKit;
using Avalonia;
using Foundation;

namespace GroupMeClientAvalonia.Notifications.Display.macOS
{
    /// <summary>
    /// Provides an adapter for <see cref="PopupNotificationProvider"/> to use Toast Notifications within the Client Window.
    /// </summary>
    public class MacOSToastNotificationProvider : IPopupNotificationSink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MacOSToastNotificationProvider"/> class.
        /// </summary>
        public MacOSToastNotificationProvider()
        {
            NSApplication.Init();
            NSApplication.Main(Environment.GetCommandLineArgs());

            // Trigger a local notification after the time has elapsed
            var notification = new NSUserNotification();

            // Add text and sound to the notification
            notification.Title = "25 Minutes is up!";
            notification.InformativeText = "Add your task to your activity log";
            notification.SoundName = NSUserNotification.NSUserNotificationDefaultSoundName;
            notification.HasActionButton = true;
            NSUserNotificationCenter.DefaultUserNotificationCenter.DeliverNotification(notification);
        }

        private GroupMeClientApi.GroupMeClient GroupMeClient { get; set; }

        /// <inheritdoc/>
        async Task IPopupNotificationSink.ShowNotification(string title, string body, string avatarUrl, bool roundedAvatar)
        {
        }

        /// <inheritdoc/>
        async Task IPopupNotificationSink.ShowLikableImageMessage(string title, string body, string avatarUrl, bool roundedAvatar, string imageUrl)
        {
        }

        /// <inheritdoc/>
        async Task IPopupNotificationSink.ShowLikableMessage(string title, string body, string avatarUrl, bool roundedAvatar)
        {
        }

        /// <inheritdoc/>
        void IPopupNotificationSink.RegisterClient(GroupMeClientApi.GroupMeClient client)
        {
            this.GroupMeClient = client;
        }
    }
}
