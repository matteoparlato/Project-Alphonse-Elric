using Helpers;
using Project_Alphonse_Elric.Core.Helpers;
using Project_Alphonse_Elric.Views;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Project_Alphonse_Elric.Dialogs
{
    public sealed partial class VoicemailNotificationEditorDialog : ContentDialog
    {
        /// <summary>
        /// Parameterless constructor of VoicemailNotificationEditorDialog class.
        /// </summary>
        public VoicemailNotificationEditorDialog()
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
                await Singleton<ClientExtensions>.Instance.GetNotifications();
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
                switch(TypeComboBox.SelectedIndex)
                {
                    case 0:
                        {
                            await Singleton<ClientExtensions>.Instance.AddNotification("report", WebUtility.HtmlEncode(MailTextBox.Text));
                            break;
                        }
                    case 1:
                        {
                            await Singleton<ClientExtensions>.Instance.AddNotification("attachment", WebUtility.HtmlEncode(MailTextBox.Text));
                            break;
                        }
                    default:
                        {
                            return;
                        }
                }
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
            Core.Models.VoicemailNotification notification = (Core.Models.VoicemailNotification)element.DataContext;

            try
            {
                await Singleton<ClientExtensions>.Instance.DeleteNotification(WebUtility.HtmlEncode(notification.Mail));
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
            AddButton.IsEnabled = new EmailAddressAttribute().IsValid(MailTextBox.Text);
        }
    }
}
