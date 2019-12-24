using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GroupMeClientAvalonia.Notifications;
using GroupMeClientAvalonia.Notifications.Display;
using GroupMeClientAvalonia.Notifications.Display.WpfToast;
using GroupMeClientAvalonia.Plugins;
using MicroCubeAvalonia.Controls;
using MicroCubeAvalonia.IconPack;
using MicroCubeAvalonia.IconPack.Icons;

namespace GroupMeClientAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private AvaloniaList<HamburgerMenuItem> menuItems = new AvaloniaList<HamburgerMenuItem>();
        private AvaloniaList<HamburgerMenuItem> menuOptionItems = new AvaloniaList<HamburgerMenuItem>();
        private HamburgerMenuItem selectedItem;
        private int unreadCount;
        private bool isReconnecting;
        private bool isRefreshing;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.InitializeClient();
        }

        /// <summary>
        /// Gets or sets the list of main items shown in the hamburger menu.
        /// </summary>
        public AvaloniaList<HamburgerMenuItem> MenuItems
        {
            get => this.menuItems;
            set => this.Set(() => this.MenuItems, ref this.menuItems, value);
        }

        /// <summary>
        /// Gets or sets the list of options items shown in the hamburger menu (at the bottom).
        /// </summary>
        public AvaloniaList<HamburgerMenuItem> MenuOptionItems
        {
            get => this.menuOptionItems;
            set => this.Set(() => this.MenuOptionItems, ref this.menuOptionItems, value);
        }

        /// <summary>
        /// Gets or sets the currently selected Hamburger Menu Tab.
        /// </summary>
        public HamburgerMenuItem SelectedItem
        {
            get => this.selectedItem;
            set => this.Set(() => this.SelectedItem, ref this.selectedItem, value);
        }

        /// <summary>
        /// Gets or sets the number of unread notifications that should be displayed in the
        /// taskbar badge.
        /// </summary>
        public int UnreadCount
        {
            get => this.unreadCount;
            set => this.Set(() => this.UnreadCount, ref this.unreadCount, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is currently reconnecting to GroupMe.
        /// </summary>
        public bool IsReconnecting
        {
            get => this.isReconnecting;
            set => this.Set(() => this.IsReconnecting, ref this.isReconnecting, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is currently refreshing.
        /// </summary>
        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.Set(() => this.IsRefreshing, ref this.isRefreshing, value);
        }

        /// <summary>
        /// Gets or sets the popup manager to be used for popups 
        /// </summary>
        public Controls.PopupViewModel PopupManager { get; set; }

        /// <summary>
        /// Gets or sets the command to be performed to refresh all displayed messages and groups.
        /// </summary>
        public ICommand RefreshEverythingCommand { get; set; }

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

        private Caching.CacheContext CacheContext { get; set; }

        private Settings.SettingsManager SettingsManager { get; set; }

        private NotificationRouter NotificationRouter { get; set; }

        //private UpdateAssist UpdateAssist { get; set; }

        private ChatsViewModel ChatsViewModel { get; set; }

        private SearchViewModel SearchViewModel { get; set; }

        private SettingsViewModel SettingsViewModel { get; set; }

        private LoginViewModel LoginViewModel { get; set; }

        private ProgressRing UpdatingSpinner { get; } = new ProgressRing() { IsActive = true, Width = 16, Foreground = Brushes.White };

        private Avalonia.Controls.Button RefreshButton { get; set; }

        private PackIconMaterialKind RefreshButtonIcon { get; set; }

        private int DisconnectedComponentCount { get; set; }

        private void InitializeClient()
        {
            Directory.CreateDirectory(this.DataRoot);

            this.SettingsManager = new Settings.SettingsManager(this.SettingsPath);
            this.SettingsManager.LoadSettings();

            PluginManager.Instance.LoadPlugins(this.PluginsPath);

            Messenger.Default.Register<Messaging.UnreadRequestMessage>(this, this.UpdateNotificationCount);
            Messenger.Default.Register<Messaging.DisconnectedRequestMessage>(this, this.UpdateDisconnectedComponentsCount);
            Messenger.Default.Register<Messaging.IndexAndRunPluginRequestMessage>(this, this.IndexAndRunCommand);

            if (string.IsNullOrEmpty(this.SettingsManager.CoreSettings.AuthToken))
            {
                // Startup in Login Mode
                this.LoginViewModel = new LoginViewModel(this.SettingsManager)
                {
                    LoginCompleted = new RelayCommand(this.InitializeClient),
                };

                this.CreateMenuItemsLoginOnly();
            }
            else
            {
                // Startup Regularly
                this.GroupMeClient = new GroupMeClientApi.GroupMeClient(this.SettingsManager.CoreSettings.AuthToken);
                this.CacheContext = new Caching.CacheContext(this.CachePath);
                this.GroupMeClient.ImageDownloader = new GroupMeClientApi.CachedImageDownloader(this.ImageCachePath);

                this.NotificationRouter = new NotificationRouter(this.GroupMeClient);

                this.ChatsViewModel = new ChatsViewModel(this.GroupMeClient, this.SettingsManager);
                this.SearchViewModel = new SearchViewModel(this.GroupMeClient, this.CacheContext);
                this.SettingsViewModel = new SettingsViewModel(this.SettingsManager);

                this.RegisterNotifications();

                this.CreateMenuItemsRegular();
            }

            Messenger.Default.Register<Messaging.DialogRequestMessage>(this, this.OpenBigPopup);

            this.PopupManager = new Controls.PopupViewModel()
            {
                ClosePopup = new RelayCommand(this.CloseBigPopup),
                EasyClosePopup = new RelayCommand(this.CloseBigPopup)
            };

            this.RefreshEverythingCommand = new RelayCommand(async() => await this.RefreshEverything(), true);

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

        private void CreateMenuItemsRegular()
        {
            this.MenuItems.Clear();
            this.MenuOptionItems.Clear();

            var chatsTab = new HamburgerMenuItem()
            {
                Icon = new IconControl() { BindableKind = PackIconMaterialKind.MessageText },
                Label = "Chats",
                ToolTip = "View Groups and Chats.",
                Tag = this.ChatsViewModel,
            };

            var secondTab = new HamburgerMenuItem()
            {
                Icon = new IconControl() { BindableKind = PackIconMaterialKind.EmailSearch },
                Label = "Search",
                ToolTip = "Search all Groups and Chats.",
                Tag = this.SearchViewModel,
            };

            var settingsTab = new HamburgerMenuItem()
            {
                Icon = new IconControl() { BindableKind = PackIconMaterialKind.SettingsOutline },
                Label = "Settings",
                ToolTip = "GroupMe Settings",
                Tag = this.SettingsViewModel,
            };

            // Add new Tabs
            this.MenuItems.Add(chatsTab);
            this.MenuItems.Add(secondTab);

            // Add new Options
            this.MenuOptionItems.Add(settingsTab);

            // Set the section to the Chats tab
            this.SelectedItem = chatsTab;
        }

        private void CreateMenuItemsLoginOnly()
        {
            this.MenuItems.Clear();
            this.MenuOptionItems.Clear();

            var loginTab = new HamburgerMenuItem()
            {
                Icon = new IconControl() { BindableKind = PackIconMaterialKind.Login },
                Label = "Login",
                ToolTip = "Login To GroupMe",
                Tag = this.LoginViewModel,
            };

            this.MenuItems.Add(loginTab);
            this.SelectedItem = loginTab;
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

        private void UpdateNotificationCount(Messaging.UnreadRequestMessage update)
        {
            this.UnreadCount = update.Count;
        }

        private void UpdateDisconnectedComponentsCount(Messaging.DisconnectedRequestMessage update)
        {
            this.DisconnectedComponentCount += update.Disconnected ? 1 : -1;
            this.DisconnectedComponentCount = Math.Max(this.DisconnectedComponentCount, 0); // make sure it never goes negative
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                this.IsReconnecting = this.DisconnectedComponentCount > 0;
            });
        }

        private void IndexAndRunCommand(Messaging.IndexAndRunPluginRequestMessage cmd)
        {
            this.SearchViewModel.ActivatePluginOnLoad = cmd.Plugin;
            this.SearchViewModel.ActivatePluginForGroupOnLoad = cmd.MessageContainer;

            // Find Search Tab entry and set as active
            foreach (var menuItem in this.MenuItems)
            {
                if (menuItem.Tag == this.SearchViewModel)
                {
                    this.SelectedItem = menuItem;
                    break;
                }
            }
        }

        private async Task RefreshEverything()
        {
            this.IsRefreshing = true;
            await this.ChatsViewModel.RefreshEverything();
            this.IsRefreshing = false;
        }
    }
}
