using Helpers;
using Microsoft.AppCenter.Analytics;
using Microsoft.Services.Store.Engagement;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Project_Alphonse_Elric.Dialogs;
using Project_Alphonse_Elric.Helpers;
using Project_Alphonse_Elric.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;
using Windows.Storage;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Project_Alphonse_Elric.Views
{
    public sealed partial class ShellPage : Page, INotifyPropertyChanged
    {
        #region ShellPage

        private const string PanoramicStateName = "PanoramicState";
        private const string WideStateName = "WideState";
        private const string NarrowStateName = "NarrowState";
        private const double WideStateMinWindowWidth = 640;
        private const double PanoramicStateMinWindowWidth = 1024;

        private bool _isPaneOpen;

        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set { Set(ref _isPaneOpen, value); }
        }

        private object _selected;

        public object Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        private SplitViewDisplayMode _displayMode = SplitViewDisplayMode.CompactInline;

        public SplitViewDisplayMode DisplayMode
        {
            get { return _displayMode; }
            set { Set(ref _displayMode, value); }
        }

        private object _lastSelectedItem;

        private ObservableCollection<ShellNavigationItem> _primaryItems = new ObservableCollection<ShellNavigationItem>();

        public ObservableCollection<ShellNavigationItem> PrimaryItems
        {
            get { return _primaryItems; }
            set { Set(ref _primaryItems, value); }
        }

        private ObservableCollection<ShellNavigationItem> _secondaryItems = new ObservableCollection<ShellNavigationItem>();

        public ObservableCollection<ShellNavigationItem> SecondaryItems
        {
            get { return _secondaryItems; }
            set { Set(ref _secondaryItems, value); }
        }

        public ShellPage()
        {
            InitializeComponent();
            DataContext = this;
            Initialize();
        }
        
        private void Initialize()
        {
            NavigationService.Frame = ShellFrame;
            NavigationService.Navigated += Frame_Navigated;
            PopulateNavItems();

            UIExtensions.SetTitleBarColor();

            AppNotification = InAppNotification;

            Loader = LoadingControl;

            Loader.IsLoading = true;

            InitializeState(Window.Current.Bounds.Width);

            if (((ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion) & 0x00000000FFFF0000L) >> 16) == 14393)
            {
                DropShadowHeader.Visibility = Visibility.Collapsed;
            }

            if (UIExtensions.IsFluentAvailable)
            {
                NavigationMenu.Style = this.Resources["AcrylicWideHamburgerMenuStyle"] as Style;
                CollectorGrid.Background = this.Resources["SystemControlChromeMediumLowAcrylicWindowMediumBrush"] as Brush;
                Loader.Background = this.Resources["SystemControlChromeMediumLowAcrylicWindowMediumBrush"] as Brush;
                LoginGrid.Background = Application.Current.Resources["CardBackground"] as Brush;
                WideNavigation.Background = this.Resources["SystemControlAccentAcrylicWindowAccentMediumHighBrush"] as Brush;
                //ConsumesAppBarButton.Style = this.Resources["AppBarButtonRevealStyle"] as Style;
                //OptionsAppBarButton.Style = this.Resources["AppBarButtonRevealStyle"] as Style;
                //VoicemailAppBarButton.Style = this.Resources["AppBarButtonRevealStyle"] as Style;
                //RechargeAppBarButton.Style = this.Resources["AppBarButtonRevealStyle"] as Style;
                //SettingsAppBarButton.Style = this.Resources["AppBarButtonRevealStyle"] as Style;
            }
            else
            {
                DropShadowContent.ShadowOpacity = 0;
            }
        }

        private void InitializeState(double windowWith)
        {
            if (windowWith < WideStateMinWindowWidth)
            {
                GoToState(NarrowStateName);
            }
            else if (windowWith < PanoramicStateMinWindowWidth)
            {
                GoToState(WideStateName);
            }
            else
            {
                GoToState(PanoramicStateName);
            }
        }

        private void PopulateNavItems()
        {
            _primaryItems.Clear();

            _primaryItems.Add(ShellNavigationItem.FromType<MainPage>("I tuoi consumi", ''));
            _primaryItems.Add(ShellNavigationItem.FromType<ServicesPage>("Opzioni e servizi", ''));
            _primaryItems.Add(ShellNavigationItem.FromType<VoicemailPage>("Segreteria telefonica", ''));
            _primaryItems.Add(ShellNavigationItem.FromType<RechargePage>("Ricarica telefonica", ''));

            _secondaryItems.Clear();

            _secondaryItems.Add(ShellNavigationItem.FromType<SettingsPage>("Impostazioni", ''));
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            var navigationItem = PrimaryItems?.FirstOrDefault(i => i.PageType == e?.SourcePageType);
            if (navigationItem == null)
            {
                navigationItem = SecondaryItems?.FirstOrDefault(i => i.PageType == e?.SourcePageType);
            }

            if (navigationItem != null)
            {
                ChangeSelected(_lastSelectedItem, navigationItem);
                _lastSelectedItem = navigationItem;
            }
        }

        private void ChangeSelected(object oldValue, object newValue)
        {
            if (oldValue != null)
            {
                (oldValue as ShellNavigationItem).IsSelected = false;
            }

            if (newValue != null)
            {
                (newValue as ShellNavigationItem).IsSelected = true;
                Selected = newValue;
            }
        }

        private void Navigate(object item)
        {
            var navigationItem = item as ShellNavigationItem;
            if (navigationItem != null)
            {
                NavigationService.Navigate(navigationItem.PageType);
            }
        }

        private void ItemClicked(object sender, ItemClickEventArgs e)
        {
            if (DisplayMode == SplitViewDisplayMode.CompactOverlay || DisplayMode == SplitViewDisplayMode.Overlay)
            {
                IsPaneOpen = false;
            }

            Navigate(e.ClickedItem);
        }

        private void OpenPane_Click(object sender, RoutedEventArgs e)
        {
            IsPaneOpen = !_isPaneOpen;
        }

        private void WindowStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e) => GoToState(e.NewState.Name);

        private void GoToState(string stateName)
        {
            switch (stateName)
            {
                case PanoramicStateName:
                    DisplayMode = SplitViewDisplayMode.Overlay;
                    if (UIExtensions.IsFluentAvailable) ShellGrid.Margin = new Thickness { Left = 0, Top = 0, Right = 0, Bottom = 0 };
                    break;
                case WideStateName:
                    DisplayMode = SplitViewDisplayMode.Overlay;
                    IsPaneOpen = false;
                    if (UIExtensions.IsFluentAvailable) ShellGrid.Margin = new Thickness { Left = 0, Top = 0, Right = 0, Bottom = 0 };
                    break;
                case NarrowStateName:
                    DisplayMode = SplitViewDisplayMode.Overlay;
                    IsPaneOpen = false;
                    if (UIExtensions.IsFluentAvailable) ShellGrid.Margin = new Thickness { Left = 0, Top = 32, Right = 0, Bottom = 0 };
                    break;
                default:
                    break;
            }
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
        private async void Page_Loaded(object sender, RoutedEventArgs e)
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
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                UIExtensions.ShowProgressStatusBar("Sto eseguendo l'accesso...");
            }
            else
            {
                StatusProgressBar.Visibility = Visibility.Visible;
            }
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
            UIExtensions.HideProgressStatusBar();
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
    }
}
