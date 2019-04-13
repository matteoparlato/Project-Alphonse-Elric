using Windows.UI.Xaml.Controls;

namespace Project_Alphonse_Elric.Dialogs
{
    /// <summary>
    /// About class
    /// </summary>
    public sealed partial class AboutDialog : ContentDialog
    {
        /// <summary>
        /// Parameterless constructor of About class.
        /// </summary>
        public AboutDialog()
        {
            this.InitializeComponent();

            //VersionTextBlock.Text = string.Format(" {0}.{1}.{2}.{3}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision);
        }
    }
}
