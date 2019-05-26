using Helpers;
using Models;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Project_Alphonse_Elric.Views
{
    /// <summary>
    /// MainPage class
    /// </summary>
    public sealed partial class MainPage : Page
    {
        internal Profile AccountDetails { get; private set; } = ClientExtensions.AccountDetails;

        /// <summary>
        /// Parameterless constructor of MainPage class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Method invoked once navigated to the page.
        /// Shows user consumes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TitleText.Text = Window.Current.Bounds.Width < 640 ? string.Format("CIAO {0}!", AccountDetails.Name.ToUpper()) : string.Format("Ciao {0}!", AccountDetails.Name);

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            Frame.BackStack.Clear();

            try
            {
                await ClientExtensions.GetConsumes();
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }

            try
            {
                DispatcherTimer updateTimeout = new DispatcherTimer() { Interval = TimeSpan.FromMinutes(1) };
                updateTimeout.Tick += (s, o) =>
                {
                    ClientExtensions.GetConsumes(); // Never await this method!
                };
                updateTimeout.Start();
            }
            catch (Exception) { }
        }

        private void WindowStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            switch (e.NewState.Name)
            {
                case "PanoramicState":
                case "WideState":
                    {
                        TitleText.Text = string.Format("Ciao {0}!", AccountDetails.Name);
                        break;
                    }
                default:
                    {
                        TitleText.Text = string.Format("CIAO {0}!", AccountDetails.Name.ToUpper());
                        break;
                    }
            }
        }
    }
}
