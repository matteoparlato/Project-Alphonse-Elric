using Helpers;
using Project_Alphonse_Elric.Core.Helpers;
using Project_Alphonse_Elric.Core.Models;
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
        /// Shows user consumes.
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
        }
    }
}
