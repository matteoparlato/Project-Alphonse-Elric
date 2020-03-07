using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Project_Alphonse_Elric.Dialogs
{
    /// <summary>
    /// PrivacyPolicyDialog class
    /// </summary>
    public sealed partial class PrivacyPolicyDialog : ContentDialog
    {
        /// <summary>
        /// Parameterless constructor of PrivacyPolicyDialog class.
        /// </summary>
        public PrivacyPolicyDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Method invoked when the user clicks on Accetta button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("AnalyticsDisabled"))
            {
                ApplicationData.Current.LocalSettings.Values.Remove("AnalyticsDisabled");
            }
        }

        /// <summary>
        /// /// Method invoked when the user clicks on Rifiuta button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("AnalyticsDisabled"))
            {
                ApplicationData.Current.LocalSettings.Values.Add("AnalyticsDisabled", true);
            }
        }
    }
}
