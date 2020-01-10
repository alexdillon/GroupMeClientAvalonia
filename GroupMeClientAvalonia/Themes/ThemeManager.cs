using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;

namespace GroupMeClientAvalonia.Themes
{
    /// <summary>
    /// <see cref="ThemeManager"/> provides support for changing the GroupMe Desktop Client Avalonia theme at runtime.
    /// </summary>
    public class ThemeManager
    {
        private static readonly StyleInclude AvaloniaLightTheme = new StyleInclude(new Uri("resm:Styles?assembly=GroupMeClientAvalonia"))
        {
            Source = new Uri("avares://Avalonia.Themes.Default/Accents/BaseLight.xaml"),
        };

        private static readonly StyleInclude AvaloniaDarkTheme = new StyleInclude(new Uri("resm:Styles?assembly=GroupMeClientAvalonia"))
        {
            Source = new Uri("avares://Avalonia.Themes.Default/Accents/BaseDark.xaml"),
        };

        private static readonly StyleInclude GroupMeLightTheme = new StyleInclude(new Uri("resm:Styles?assembly=GroupMeClientAvalonia"))
        {
            Source = new Uri("avares://GroupMeClientAvalonia/GroupMeLight.xaml"),
        };

        private static readonly StyleInclude GroupMeDarkTheme = new StyleInclude(new Uri("resm:Styles?assembly=GroupMeClientAvalonia"))
        {
            Source = new Uri("avares://GroupMeClientAvalonia/GroupMeDark.xaml"),
        };

        /// <summary>
        /// Gets the style dictionary associated with the current base Avalonia theme.
        /// </summary>
        public static StyleInclude CurrentAvaloniaTheme { get; private set; }

        /// <summary>
        /// Gets the style dictionary associated with the current GroupMe theme.
        /// </summary>
        public static StyleInclude CurrentGroupMeTheme { get; private set; }

        private static bool IsInitialized { get; set; }

        private static bool IsPending { get; set; }

        public static void Initialize()
        {
            Program.GroupMeMainWindow.Styles.Add(AvaloniaLightTheme);
            Program.GroupMeMainWindow.Styles.Add(GroupMeLightTheme);
            IsInitialized = true;

            if (IsPending)
            {
                // If any theme change requests were submitted prior to initialization,
                // apply them now.
                ApplyTheme();
                IsPending = false;
            }
        }

        public static void SetLightTheme()
        {
            CurrentAvaloniaTheme = AvaloniaLightTheme;
            CurrentGroupMeTheme = GroupMeLightTheme;

            if (IsInitialized)
            {
                ApplyTheme();
            }
            else
            {
                IsPending = true;
            }
        }

        public static void SetDarkTheme()
        {
            CurrentAvaloniaTheme = AvaloniaDarkTheme;
            CurrentGroupMeTheme = GroupMeDarkTheme;

            if (IsInitialized)
            {
                ApplyTheme();
            }
            else
            {
                IsPending = true;
            }
        }

        private static void ApplyTheme()
        {
            Program.GroupMeMainWindow.Styles[0] = CurrentAvaloniaTheme;
            Program.GroupMeMainWindow.Styles[1] = CurrentGroupMeTheme;
        }
    }
}
