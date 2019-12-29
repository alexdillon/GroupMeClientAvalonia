using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using System;

namespace GroupMeClientAvalonia.Themes
{
    public class ThemeManager
    {
        private readonly static StyleInclude AvaloniaLightTheme = new StyleInclude(new Uri("resm:Styles?assembly=GroupMeClientAvalonia"))
        {
            Source = new Uri("avares://Avalonia.Themes.Default/Accents/BaseLight.xaml")
        };

        private readonly static StyleInclude AvaloniaDarkTheme = new StyleInclude(new Uri("resm:Styles?assembly=GroupMeClientAvalonia"))
        {
            Source = new Uri("avares://Avalonia.Themes.Default/Accents/BaseDark.xaml")
        };

        private readonly static StyleInclude GroupMeLightTheme = new StyleInclude(new Uri("resm:Styles?assembly=GroupMeClientAvalonia"))
        {
            Source = new Uri("avares://GroupMeClientAvalonia/GroupMeLight.xaml")
        };

        private readonly static StyleInclude GroupMeDarkTheme = new StyleInclude(new Uri("resm:Styles?assembly=GroupMeClientAvalonia"))
        {
            Source = new Uri("avares://GroupMeClientAvalonia/GroupMeDark.xaml")
        };

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

        public static StyleInclude CurrentAvaloniaTheme { get; private set; }

        public static StyleInclude CurrentGroupMeTheme { get; private set; }
    }
}
