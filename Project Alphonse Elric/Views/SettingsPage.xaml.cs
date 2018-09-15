using System;
using Helpers;
using Microsoft.Services.Store.Engagement;
using Models;
using Project_Alphonse_Elric.Dialogs;
using Project_Alphonse_Elric.Helpers;
using Project_Alphonse_Elric.Services;
using Windows.Devices.Enumeration;
using Windows.Devices.Power;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Project_Alphonse_Elric.Views
{
    /// <summary>
    /// SettingsPage class
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        /// <summary>
        /// Parameterless constructor of SettingsPage class.
        /// </summary>
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Method invoked once navigated to the page.
        /// Loads app settings state and checks if Windows Hello is available in the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("HelloAuthenticationEnabled"))
            {
                WindowsHelloCheckBox.IsChecked = true;
            }

            if (!await KeyCredentialManager.IsSupportedAsync())
            {
                WindowsHelloCheckBox.IsEnabled = false;
                WindowsHelloRelativePanel.Visibility = Visibility.Visible;
            }
            else
            {
                WindowsHelloRelativePanel.Visibility = Visibility.Collapsed;
            }

            DeviceInformationCollection batteryCollection = await DeviceInformation.FindAllAsync(Battery.GetDeviceSelector());
            BatterySaverRelativePanel.Visibility = batteryCollection.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Method invoked when the user clicks on Invia un feedback.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
        }

        /// <summary>
        /// Method invoked when the user clicks on Informazioni sull'app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            await new AboutDialog().ShowAsync();
        }

        /// <summary>
        /// Method invoked when the user clicks on Cambia utente.
        /// Unregister the background task and removes all user data in order to allow
        /// another user to use the app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                await ClientExtensions.Logout();

                ShellPage.Current.SessionTimeout.Stop();

                SecurityExtensions.RemoveCredentials();

                ClientExtensions.AccountDetails = new Profile();

                NavigationService.Navigate(typeof(LockedPage));

                ShellPage.Current.Loader.IsLoading = true;
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        #region Windows Hello

        /// <summary>
        /// Method invoked when the user clicks on Usa Windows Hello.
        /// Checks user's trustworthiness and enable Windows Hello.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (WindowsHelloCheckBox.IsChecked == true)
            {
                if (await UserConsentVerifier.RequestVerificationAsync(SecurityExtensions.MESSAGE) == UserConsentVerificationResult.Verified)
                {
                    WindowsHelloCheckBox.IsChecked = true;

                    SecurityExtensions.RegisterKey();
                }
                else
                {
                    WindowsHelloCheckBox.IsChecked = false;

                    SecurityExtensions.RemoveKey();
                }
            }
            else
            {
                SecurityExtensions.RemoveKey();
            }
        }

        /// <summary>
        /// Method invoked when the Open Windows Settings button is pressed.
        /// Opens the Settings page for managing sign-in options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:signinoptions"));
        }

        #endregion

        #region Notifications and live tile

        /// <summary>
        /// Method called when the Add Exception button is pressed.
        /// Opens the Settings page for battery save mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OpenSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:batterysaver-usagedetails"));
        }

        /// <summary>
        /// Method invoked when the Gestisci app in background button is pressed.
        /// Opens the Settings page for managing background apps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OpenPrivacyBackgroundSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-backgroundapps"));
        }

        #endregion
    }
}
