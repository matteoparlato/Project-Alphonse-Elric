using Helpers;
using Project_Alphonse_Elric.Core.Helpers;
using Project_Alphonse_Elric.Views;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Project_Alphonse_Elric.Dialogs
{
    /// <summary>
    /// FastCallsEditor class
    /// </summary>
    public sealed partial class FastCallsEditorDialog : ContentDialog
    {
        /// <summary>
        /// Parameterless constructor of FastCallsEditorDialog class.
        /// </summary>
        public FastCallsEditorDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Method invoked once the dialog opened.
        /// Shows user defined shortenings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Singleton<ClientExtensions>.Instance.GetFastCalls();
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }

            LoadingProgressRing.IsActive = false;

            NoDataStackPanel.Opacity = Singleton<ClientExtensions>.Instance.AccountDetails.ActiveServices.FastCallList.Count > 0 ? 0 : 1;
            DataListView.ItemsSource = Singleton<ClientExtensions>.Instance.AccountDetails.ActiveServices.FastCallList;
        }

        /// <summary>
        /// Method invoked when the user clicks on + button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                await Singleton<ClientExtensions>.Instance.AddFastCall(NameTextBox.Text, TargetTextBox.Text, ShortTextBox.Text);
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }

            ContentDialog_Loaded(null, null);
        }

        /// <summary>
        /// Method invoked when the user clicks on Elimina hyperlink.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Core.Models.FastCall fastcall = (Core.Models.FastCall)element.DataContext;

            try
            {
                await Singleton<ClientExtensions>.Instance.DeleteFastCall(fastcall.DeleteLink);
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }

            ContentDialog_Loaded(null, null);
        }

        /// <summary>
        /// Method invoked when the user writes inside dialogs textboxes.
        /// Checks if user input is correct.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddButton.IsEnabled = !string.IsNullOrWhiteSpace(NameTextBox.Text) &&
                                  !string.IsNullOrWhiteSpace(TargetTextBox.Text) &&
                                  !string.IsNullOrWhiteSpace(ShortTextBox.Text) &&
                                  ShortTextBox.Text.Length > 3 &&
                                  ShortTextBox.Text.Length < 11 &&
                                  !ShortTextBox.Text.StartsWith('0');
        }
    }
}
