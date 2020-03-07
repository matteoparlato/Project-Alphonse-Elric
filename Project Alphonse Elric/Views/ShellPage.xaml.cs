using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Helpers;
using Microsoft.AppCenter.Analytics;
using Microsoft.Services.Store.Engagement;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using Project_Alphonse_Elric.Core.Helpers;
using Project_Alphonse_Elric.Core.Models;
using Project_Alphonse_Elric.Dialogs;
using Project_Alphonse_Elric.Helpers;
using Project_Alphonse_Elric.Services;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

using WinUI = Microsoft.UI.Xaml.Controls;

namespace Project_Alphonse_Elric.Views
{
    public sealed partial class ShellPage : Page, INotifyPropertyChanged
    {
        #region ShellPage

        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);

        private bool _isBackEnabled;
        private WinUI.NavigationViewItem _selected;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { Set(ref _isBackEnabled, value); }
        }

        public WinUI.NavigationViewItem Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ShellPage()
        {
            InitializeComponent();
            DataContext = this;
            Initialize();
        }

        public string PaneTitle
        {
            get { return ResourceLoader.GetForCurrentView().GetString("AppName"); }
        }

        private void Initialize()
        {
            NavigationService.Frame = shellFrame;
            NavigationService.NavigationFailed += Frame_NavigationFailed;
            NavigationService.Navigated += Frame_Navigated;
            navigationView.BackRequested += OnBackRequested;

            //
            ApplicationView view = ApplicationView.GetForCurrentView();
            view.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            navigationView.ItemInvoked += OnItemInvoked;

            AppNotification = AppNotificationTeachingTip;

            Loader = LoadingControl;

            Loader.IsLoading = true;
            //
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            KeyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            KeyboardAccelerators.Add(_backKeyboardAccelerator);
            await Task.CompletedTask;

            //
            Page_Loaded();
            //
        }
        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = navigationView.SettingsItem as WinUI.NavigationViewItem;
                return;
            }

            Selected = navigationView.MenuItems
                            .OfType<WinUI.NavigationViewItem>()
                            .FirstOrDefault(menuItem => IsMenuItemForPageType(menuItem, e.SourcePageType));
        }

        private bool IsMenuItemForPageType(WinUI.NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        private void OnItemInvoked(WinUI.NavigationView sender, WinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationService.Navigate(typeof(SettingsPage));
                return;
            }
            else if (args.InvokedItemContainer != null)
            {

                var item = navigationView.MenuItems
                            .OfType<WinUI.NavigationViewItem>()
                            .First(menuItem => (string)menuItem.Content == (string)args.InvokedItem);
                var pageType = item.GetValue(NavHelper.NavigateToProperty) as Type;
                NavigationService.Navigate(pageType, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void OnBackRequested(WinUI.NavigationView sender, WinUI.NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.GoBack();
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        internal static ShellPage Current { get; set; }

        internal TeachingTip AppNotification { get; private set; }

        internal Loading Loader { get; private set; }

        internal DispatcherTimer SessionTimeout = new DispatcherTimer() { Interval = TimeSpan.FromMinutes(5) };

        private bool _avoidCheck = false;

        internal Profile AccountDetails { get; private set; } = Singleton<ClientExtensions>.Instance.AccountDetails;

        /// <summary>
        /// Method invoked once navigated to the page.
        /// Begins user login.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded()
        {
            if (SystemInformation.IsFirstRun) await new PrivacyPolicyDialog().ShowAsync();

            PasswordCredential credentials = SecurityExtensions.RetrieveCredentials();
            if (credentials != null)
            {
                _avoidCheck = true;

                credentials.RetrievePassword();

                UsernameTextBox.Text = credentials.UserName;
                PasswordPasswordBox.Password = credentials.Password;

                Button_Click(null, null);
            }
        }

        /// <summary>
        /// Method invoked when the user clicks on Accedi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StatusProgressBar.Opacity = 1;
            UsernameTextBox.IsEnabled = false;
            PasswordPasswordBox.IsEnabled = false;
            LoginButton.IsEnabled = false;

            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("HelloAuthenticationEnabled"))
                {
                    switch (await UserConsentVerifier.RequestVerificationAsync(SecurityExtensions.MESSAGE))
                    {
                        case UserConsentVerificationResult.Verified:
                            {
                                await Singleton<ClientExtensions>.Instance.Authenticate(UsernameTextBox.Text, PasswordPasswordBox.Password);
                                break;
                            }
                        case UserConsentVerificationResult.DeviceNotPresent:
                            {
                                SecurityExtensions.RemoveCredentials();
                                await new MessageDialog("L'autenticazione sicura tramite Windows Hello è stata disabilitata a causa della mancanza di opzioni di accesso valide. Per poter riabilitare questa funzione è necessario specificare un metodo di autenticazione valido (PIN, impronta digitale o scansione dell'iride) nelle impostazioni di Windows.\n\nPer motivi di sicurezza le credenziali del tuo account iliad dovranno essere inserite manualmente per questa sessione.", "Attenzione").ShowAsync();
                                break;
                            }
                        default:
                            {
                                Application.Current.Exit();
                                break;
                            }
                    }
                }
                else
                {
                    await Singleton<ClientExtensions>.Instance.Authenticate(UsernameTextBox.Text, PasswordPasswordBox.Password);
                }

                SessionTimeout.Tick += (s, o) =>
                {
                    Singleton<ClientExtensions>.Instance.Authenticate(UsernameTextBox.Text, PasswordPasswordBox.Password); // Never await this method!
                };

                SecurityExtensions.AddCredentials(UsernameTextBox.Text, PasswordPasswordBox.Password);

                AppNotification.IsOpen = false;

                NavigationService.Navigate(typeof(MainPage), new EntranceNavigationTransitionInfo());
                navigationView.SelectedItem = navigationView.MenuItems[0];
                IsBackEnabled = false;
                NavigationService.Frame.BackStack.Clear();

                Loader.IsLoading = false;

                SessionTimeout.Start();
            }
            catch (Exception ex)
            {
                HandleExceptionNotification(ex);
            }

            _avoidCheck = false;

            UsernameTextBox.IsEnabled = true;
            PasswordPasswordBox.IsEnabled = true;
            LoginButton.IsEnabled = true;
            StatusProgressBar.Opacity = 0;
        }

        /// <summary>
        /// Method invoked when the text in the UsernameTextBox changed.
        /// Sets LoginButton enabled or disabled state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_avoidCheck)
            {
                LoginButton.IsEnabled = !string.IsNullOrWhiteSpace(UsernameTextBox.Text) && !string.IsNullOrWhiteSpace(PasswordPasswordBox.Password);
            }
        }

        /// <summary>
        /// Method invoked when the text in the PasswordPasswordBox changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TextBox_TextChanged(null, null);
        }

        /// <summary>
        /// Method invoked when an handled exception occurs.
        /// </summary>
        /// <param name="ex">The generated exception</param>
        public void HandleExceptionNotification(Exception ex)
        {
            Analytics.TrackEvent(ex.Message, new Dictionary<string, string> { { "exception", ex.ToString() } });

            if (ex is IndexOutOfRangeException)
            {
                AppNotification.Subtitle = "Si è verificato un errore durante l'accesso all'account iliad. Controlla le credenziali inserite e riprova. Se il problema dovesse persistere contatta lo sviluppatore dell'app.";                
            }
            else
            {
                AppNotification.Subtitle = "Impossibile comunicare con il server remoto di iliad. Verifica di avere una connessione ad Internet attiva e riprova. Se il problema dovesse persistere contatta lo sviluppatore dell'app.";
            }
            AppNotification.Title = "Attenzione";
            AppNotification.IsOpen = true;
        }

        /// <summary>
        /// Method invoked when the user clicks on Problemi di accesso al tuo accound iliad?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
        }

        /// <summary>
        /// Method invoked when the user presses a key on the keyboard.
        /// If the Enter key is pressed begin the login process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordPasswordBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter) Button_Click(null, null);
        }
    }
}
