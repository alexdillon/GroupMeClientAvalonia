﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Input;
using Avalonia;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GroupMeClientAvalonia.Notifications;
using GroupMeClientAvalonia.Notifications.Display;
using GroupMeClientAvalonia.Notifications.Display.WpfToast;

namespace GroupMeClientAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.InitializeClient();
        }



        /// <summary>
        /// Gets or sets the popup manager to be used for popups 
        /// </summary>
        public Controls.PopupViewModel PopupManager { get; set; }

        /// <summary>
        /// Gets the Toast Holder Manager for this application.
        /// </summary>
        public ToastHolderViewModel ToastHolderManager { get; private set; }

        private string DataRoot => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MicroCube", "GroupMe Desktop Client");

        private string SettingsPath => Path.Combine(this.DataRoot, "settings.json");

        private string CachePath => Path.Combine(this.DataRoot, "cache.db");

        private string ImageCachePath => Path.Combine(this.DataRoot, "ImageCache");

        private string PluginsPath => Path.Combine(this.DataRoot, "Plugins");

        private GroupMeClientApi.GroupMeClient GroupMeClient { get; set; }

        //private Caching.CacheContext CacheContext { get; set; }

        private Settings.SettingsManager SettingsManager { get; set; }

        private NotificationRouter NotificationRouter { get; set; }

        //private UpdateAssist UpdateAssist { get; set; }

            // TODO not this
        public ChatsViewModel ChatsViewModel { get; set; }

        //private SearchViewModel SearchViewModel { get; set; }

        //private SettingsViewModel SettingsViewModel { get; set; }

        //private LoginViewModel LoginViewModel { get; set; }

        //private ProgressRing ReconnectingSpinner { get; } = new ProgressRing() { IsActive = false, Width = 20, Foreground = System.Windows.Media.Brushes.White };

        //private ProgressRing UpdatingSpinner { get; } = new ProgressRing() { IsActive = true, Width = 20, Foreground = System.Windows.Media.Brushes.White };

        //private System.Windows.Controls.Button RefreshButton { get; set; }

        //private PackIconMaterial RefreshButtonIcon { get; set; }

        //private int DisconnectedComponentCount { get; set; }

        private void InitializeClient()
        {
            Directory.CreateDirectory(this.DataRoot);

            this.SettingsManager = new Settings.SettingsManager(this.SettingsPath);
            this.SettingsManager.LoadSettings();

            //PluginManager.Instance.LoadPlugins(this.PluginsPath);

            //Messenger.Default.Register<Messaging.UnreadRequestMessage>(this, this.UpdateNotificationCount);
            //Messenger.Default.Register<Messaging.DisconnectedRequestMessage>(this, this.UpdateDisconnectedComponentsCount);
            //Messenger.Default.Register<Messaging.IndexAndRunPluginRequestMessage>(this, this.IndexAndRunCommand);

            if (string.IsNullOrEmpty(this.SettingsManager.CoreSettings.AuthToken))
            {
                // Startup in Login Mode
                //this.LoginViewModel = new LoginViewModel(this.SettingsManager)
                //{
                //    LoginCompleted = new RelayCommand(this.InitializeClient),
                //};

                //this.CreateMenuItemsLoginOnly();
            }
            else
            {
                // Startup Regularly
                this.GroupMeClient = new GroupMeClientApi.GroupMeClient(this.SettingsManager.CoreSettings.AuthToken);
                //this.CacheContext = new Caching.CacheContext(this.CachePath);
                this.GroupMeClient.ImageDownloader = new GroupMeClientApi.CachedImageDownloader(this.ImageCachePath);

                this.NotificationRouter = new NotificationRouter(this.GroupMeClient);

                this.ChatsViewModel = new ChatsViewModel(this.GroupMeClient, this.SettingsManager);
                //this.SearchViewModel = new SearchViewModel(this.GroupMeClient, this.CacheContext);
                //this.SettingsViewModel = new SettingsViewModel(this.SettingsManager);

                this.RegisterNotifications();

                //this.CreateMenuItemsRegular();
            }

            Messenger.Default.Register<Messaging.DialogRequestMessage>(this, this.OpenBigPopup);

            this.PopupManager = new Controls.PopupViewModel()
            {
                ClosePopup = new RelayCommand(this.CloseBigPopup),
                EasyClosePopup = new RelayCommand(this.CloseBigPopup)
            };

            //this.UpdateAssist = new UpdateAssist();
            //Application.Current.MainWindow.Closing += new CancelEventHandler(this.MainWindow_Closing);
        }

        private void RegisterNotifications()
        {
            this.ToastHolderManager = new ToastHolderViewModel();

            this.NotificationRouter.RegisterNewSubscriber(this.ChatsViewModel);
            //this.NotificationRouter.RegisterNewSubscriber(PopupNotificationProvider.CreatePlatformNotificationProvider());
            this.NotificationRouter.RegisterNewSubscriber(PopupNotificationProvider.CreateInternalNotificationProvider(this.ToastHolderManager));
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            //if (!this.UpdateAssist.CanShutdown)
            //{
            //    this.UpdateAssist.UpdateMonitor.Task.ContinueWith(this.UpdateCompleted);
            //    e.Cancel = true;

            //    var updatingTab = new HamburgerMenuIconItem()
            //    {
            //        Icon = this.UpdatingSpinner,
            //        Label = "Updating",
            //        ToolTip = "Updating",
            //        Tag = new UpdatingViewModel(),
            //    };

            //    this.MenuItems.Add(updatingTab);
            //    this.SelectedItem = updatingTab;
            //}
        }

        private void OpenBigPopup(Messaging.DialogRequestMessage dialog)
        {
            this.PopupManager.PopupDialog = dialog.Dialog;
        }

        private void CloseBigPopup()
        {
            if (this.PopupManager.PopupDialog is IDisposable d)
            {
                d.Dispose();
            }

            this.PopupManager.PopupDialog = null;
        }
    }
}
