using AdDealsUniversalSDKW81;
using Helpers;
using Project_Alphonse_Elric.Core.Helpers;
using Project_Alphonse_Elric.Core.Models;
using Project_Alphonse_Elric.Dialogs;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Project_Alphonse_Elric.Views
{
    /// <summary>
    /// MainPage class
    /// </summary>
    public sealed partial class MainPage : Page
    {
        internal static bool AdViewed { get; private set; }

        internal Profile AccountDetails { get; private set; } = Singleton<ClientExtensions>.Instance.AccountDetails;

        /// <summary>
        /// Parameterless constructor of MainPage class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Method invoked once navigated to the page.
        /// Shows user's consumes and shows AdDialog on first MainPage navigation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Singleton<ClientExtensions>.Instance.GetConsumes();
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }

            try
            {
                DispatcherTimer updateTimeout = new DispatcherTimer() { Interval = TimeSpan.FromMinutes(1) };
                updateTimeout.Tick += (s, o) =>
                {
                    Singleton<ClientExtensions>.Instance.GetConsumes(); // Never await this method!
                };
                updateTimeout.Start();
            }
            catch (Exception) { }

            if (!AdViewed)
            {
                AdViewed = true;
                // AdJumbo ad network temporarily disabled
                // await new AdDialog().ShowAsync();
                ShellPage.Current.ShowPopupAd(AdManager.AdKind.FULLSCREENPOPUPAD);
            }
        }
    }
}
