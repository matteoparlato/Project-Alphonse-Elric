using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Project_Alphonse_Elric.Helpers
{
    /// <summary>
    /// UIExtensions class
    /// </summary>
    internal static class UIExtensions
    {
        internal static bool IsFluentAvailable
        {
            get
            {
                return ApiInformation.IsTypePresent(typeof(AcrylicBrush).FullName);
            }
        }

        /// <summary>
        /// Method which sets the titlebar color.
        /// </summary>
        internal static void SetTitleBarColor()
        {
            SolidColorBrush theme = Application.Current.Resources["SystemControlBackgroundChromeMediumLowBrush"] as SolidColorBrush;

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundOpacity = 1;

                if (theme != null) statusBar.BackgroundColor = theme.Color;
            }
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                if (IsFluentAvailable)
                {
                    CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

                    ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                    titleBar.ButtonForegroundColor = (Color)Application.Current.Resources["SystemBaseHighColor"];
                }
                else
                {
                    if (theme != null)
                    {
                        ApplicationView applicationView = ApplicationView.GetForCurrentView();

                        applicationView.TitleBar.BackgroundColor = theme.Color;
                        applicationView.TitleBar.InactiveBackgroundColor = theme.Color;
                        applicationView.TitleBar.ButtonBackgroundColor = theme.Color;
                        applicationView.TitleBar.ButtonInactiveBackgroundColor = theme.Color;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        internal static void ShowProgressStatusBar(string message)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.ProgressIndicator.Text = message;
                statusBar.ProgressIndicator.ShowAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal static void HideProgressStatusBar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.ProgressIndicator.HideAsync();
            }
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
