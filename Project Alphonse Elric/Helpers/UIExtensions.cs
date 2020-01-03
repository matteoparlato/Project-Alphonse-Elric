using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Project_Alphonse_Elric.Helpers
{
    /// <summary>
    /// UIExtensions class
    /// </summary>
    internal static class UIExtensions
    {
        /// <summary>
        /// Method which sets the titlebar color.
        /// </summary>
        internal static void SetTitleBarColor()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = (Color)Application.Current.Resources["SystemBaseHighColor"];
        }

        /// <summary>
        /// Method which sets the app theme based on system setting.
        /// </summary>
        internal static void SetTheme()
        {
            Application.Current.RequestedTheme = Application.Current.RequestedTheme == ApplicationTheme.Light ? ApplicationTheme.Light : ApplicationTheme.Dark;
        }
    }
}
