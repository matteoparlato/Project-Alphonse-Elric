using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

namespace Project_Alphonse_Elric.Views
{
    /// <summary>
    /// RechargePage class
    /// </summary>
    public sealed partial class RechargePage : Page
    {
        /// <summary>
        /// Parameterless constructor of RechargePage class.
        /// </summary>
        public RechargePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Method invoked once navigated to the page.
        /// Starts the navigation to the Ricarica il tuo numero web page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://www.iliad.it/account/ricarica")))
            {
                PageWebView.NavigateWithHttpRequestMessage(request);
            }
        }

        /// <summary>
        /// Method invoked when the WebView completes the navigation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PageWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            StatusProgressRing.IsActive = false;
        }
    }
}
