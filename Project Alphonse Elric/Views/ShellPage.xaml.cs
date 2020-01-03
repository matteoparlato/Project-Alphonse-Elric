using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Helpers;
using Microsoft.AppCenter.Analytics;
using Microsoft.Services.Store.Engagement;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Project_Alphonse_Elric.Dialogs;
using Project_Alphonse_Elric.Helpers;
using Project_Alphonse_Elric.Services;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

        private void Initialize()
        {
            NavigationService.Frame = shellFrame;
            NavigationService.NavigationFailed += Frame_NavigationFailed;
            NavigationService.Navigated += Frame_Navigated;
            navigationView.BackRequested += OnBackRequested;

            UIExtensions.SetTitleBarColor();

            AppNotification = InAppNotification;

            Loader = LoadingControl;

            Loader.IsLoading = true;

            //CollectorGrid.Background = this.Resources["SystemControlChromeMediumLowAcrylicWindowMediumBrush"] as Brush;
            Loader.Background = this.Resources["SystemControlChromeMediumLowAcrylicWindowMediumBrush"] as Brush;
            LoginGrid.Background = Application.Current.Resources["CardBackground"] as Brush;
            //WideNavigation.Background = this.Resources["SystemControlAccentAcrylicWindowAccentMediumHighBrush"] as Brush;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            KeyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            KeyboardAccelerators.Add(_backKeyboardAccelerator);
            await Task.CompletedTask;

            Page_Loaded();
        }
        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
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
            var item = navigationView.MenuItems
                            .OfType<WinUI.NavigationViewItem>()
                            .First(menuItem => (string)menuItem.Content == (string)args.InvokedItem);
            var pageType = item.GetValue(NavHelper.NavigateToProperty) as Type;
            NavigationService.Navigate(pageType);
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

        internal InAppNotification AppNotification { get; private set; }

        internal Loading Loader { get; private set; }

        internal DispatcherTimer SessionTimeout = new DispatcherTimer() { Interval = TimeSpan.FromMinutes(5) };

        private bool _avoidCheck = false;

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
            StatusProgressBar.Visibility = Visibility.Visible;
            ErrorStackPanel.Visibility = Visibility.Collapsed;
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
                                await ClientExtensions.Authenticate(UsernameTextBox.Text, PasswordPasswordBox.Password);
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
                    await ClientExtensions.Authenticate(UsernameTextBox.Text, PasswordPasswordBox.Password);
                }

                SessionTimeout.Tick += (s, o) =>
                {
                    ClientExtensions.Authenticate(UsernameTextBox.Text, PasswordPasswordBox.Password); // Never await this method!
                };

                SecurityExtensions.AddCredentials(UsernameTextBox.Text, PasswordPasswordBox.Password);

                AppNotification.Dismiss();

                NavigationService.Navigate(typeof(MainPage));

                Loader.IsLoading = false;

                SessionTimeout.Start();
            }
            catch (IndexOutOfRangeException)
            {
                ErrorTextBlock.Text = "Si è verificato un errore durante l'accesso all'account iliad. Controlla le credenziali inserite e riprova. Se il problema dovesse persistere contatta lo sviluppatore dell'app.";
                ErrorStackPanel.Visibility = Visibility.Visible;
            }
            catch (HttpRequestException)
            {
                ErrorTextBlock.Text = "Impossibile comunicare con il server remoto di iliad. Verifica di avere una connessione ad Internet attiva e riprova. Se il problema dovesse persistere contatta lo sviluppatore dell'app.";
                ErrorStackPanel.Visibility = Visibility.Visible;
            }
            catch (COMException)
            {
                ErrorTextBlock.Text = "Impossibile comunicare con il server remoto di iliad. Verifica di avere una connessione ad Internet attiva e riprova. Se il problema dovesse persistere contatta lo sviluppatore dell'app.";
                ErrorStackPanel.Visibility = Visibility.Visible;
            }

            _avoidCheck = false;

            UsernameTextBox.IsEnabled = true;
            PasswordPasswordBox.IsEnabled = true;
            LoginButton.IsEnabled = true;
            StatusProgressBar.Visibility = Visibility.Collapsed;
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
            if (ex is IndexOutOfRangeException)
            {
                AppNotification.Content = "Si è verificato un errore durante l'accesso all'account iliad. Controlla le credenziali inserite e riprova. Se il problema dovesse persistere contatta lo sviluppatore dell'app.";
                AppNotification.Show();
            }
            if (ex is HttpRequestException)
            {
                AppNotification.Content = "Impossibile comunicare con il server remoto di iliad. Verifica di avere una connessione ad Internet attiva e riprova. Se il problema dovesse persistere contatta lo sviluppatore dell'app.";
                AppNotification.Show();
            }
            if (ex is COMException)
            {
                AppNotification.Content = "Impossibile comunicare con il server remoto di iliad. Verifica di avere una connessione ad Internet attiva e riprova. Se il problema dovesse persistere contatta lo sviluppatore dell'app.";
                AppNotification.Show();
            }

            Analytics.TrackEvent(ex.Message, new Dictionary<string, string> { { "exception", ex.ToString() } });
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
        /// Method invoked when the user clicks on I tuoi consumi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(MainPage));
            NavigationService.Frame.BackStack.Clear();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Method invoked when the user clicks on Opzioni e servizi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(ServicesPage));
            NavigationService.Frame.BackStack.Clear();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Method invoked when the user clicks on Segreteria telefonica.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(VoicemailPage));
            NavigationService.Frame.BackStack.Clear();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Method invoked when the user clicks on Ricarica.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_3(object sender, RoutedEventArgs e)
        {
            AppNotification.Dismiss();
            NavigationService.Navigate(typeof(RechargePage));
            NavigationService.Frame.BackStack.Clear();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Method invoked when the user clicks on Impostazioni.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_4(object sender, RoutedEventArgs e)
        {
            AppNotification.Dismiss();
            NavigationService.Navigate(typeof(SettingsPage));
            NavigationService.Frame.BackStack.Clear();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
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




        //////////////////
        ///




        private void shellFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        // List of ValueTuple holding the Navigation Tag and the relative Navigation Page
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("home", typeof(MainPage)),
            ("services", typeof(ServicesPage)),
            ("voicemail", typeof(VoicemailPage)),
            ("recharge", typeof(RechargePage)),
            ("settings", typeof(SettingsPage)),
        };

        private void navigationView_Loaded(object sender, RoutedEventArgs e)
        {
            // Add handler for shellFrame navigation.
            shellFrame.Navigated += On_Navigated;

            // navigationView doesn't load any page by default, so load home page.
            navigationView.SelectedItem = navigationView.MenuItems[0];
            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            navigationView_Navigate("home", new EntranceNavigationTransitionInfo());

            // Add keyboard accelerators for backwards navigation.
            var goBack = new KeyboardAccelerator { Key = VirtualKey.GoBack };
            goBack.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(goBack);

            // ALT routes here
            var altLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left,
                Modifiers = VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(altLeft);
        }

        private void navigationView_ItemInvoked(WinUI.NavigationView sender,
                                         WinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                navigationView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                navigationView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        // navigationView_SelectionChanged is not used in this example, but is shown for completeness.
        // You will typically handle either ItemInvoked or SelectionChanged to perform navigation,
        // but not both.
        private void navigationView_SelectionChanged(WinUI.NavigationView sender,
                                              WinUI.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                navigationView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                navigationView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void navigationView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                _page = typeof(SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = shellFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                shellFrame.Navigate(_page, null, transitionInfo);
            }
        }

        private void navigationView_BackRequested(WinUI.NavigationView sender,
                                           WinUI.NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }

        private void BackInvoked(KeyboardAccelerator sender,
                                 KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        private bool On_BackRequested()
        {
            if (!shellFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed.
            if (navigationView.IsPaneOpen &&
                (navigationView.DisplayMode == WinUI.NavigationViewDisplayMode.Compact ||
                 navigationView.DisplayMode == WinUI.NavigationViewDisplayMode.Minimal))
                return false;

            shellFrame.GoBack();
            return true;
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            navigationView.IsBackEnabled = shellFrame.CanGoBack;

            if (shellFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of navigationView.MenuItems, and doesn't have a Tag.
                navigationView.SelectedItem = (WinUI.NavigationViewItem)navigationView.SettingsItem;
                navigationView.Header = "Settings";
            }
            else if (shellFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                navigationView.SelectedItem = navigationView.MenuItems
                    .OfType<WinUI.NavigationViewItem>()
                    .First(n => n.Tag.Equals(item.Tag));

                navigationView.Header =
                    ((WinUI.NavigationViewItem)navigationView.SelectedItem)?.Content?.ToString();
            }
        }
        /////
        //////////////////////////////
    }
}
