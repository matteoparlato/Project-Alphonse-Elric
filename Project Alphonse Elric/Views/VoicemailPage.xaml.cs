using Helpers;
using Project_Alphonse_Elric.Core.Models;
using Project_Alphonse_Elric.Dialogs;
using System;
using Windows.ApplicationModel.Calls;
using Windows.ApplicationModel.Chat;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Project_Alphonse_Elric.Views
{
    /// <summary>
    /// VoicemailPage class
    /// </summary>
    public sealed partial class VoicemailPage : Page
    {
        internal Profile AccountDetails { get; private set; } = ClientExtensions.AccountDetails;

        /// <summary>
        /// Parameterless constructor of VoicemailPage class.
        /// </summary>
        public VoicemailPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Method invoked once navigated to the page.
        /// Shows user voicemail messages and options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await ClientExtensions.GetMessages();
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }

            LoadingProgressRing.IsActive = false;

            NoDataStackPanel.Visibility = ClientExtensions.AccountDetails.Voicemail.MessagesList.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            DataGridView.ItemsSource = ClientExtensions.AccountDetails.Voicemail.MessagesList;
        }

        /// <summary>
        /// Method invoked when the user clicks on a checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            try
            {
                if ((bool)checkBox.IsChecked)
                {
                    await ClientExtensions.SendEnableRequest((string)checkBox.CommandParameter);
                }
                else
                {
                    await ClientExtensions.SendDisableRequest((string)checkBox.CommandParameter);
                }
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Personalizza notifiche hyperlink.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await new VoicemailNotificationEditor().ShowAsync();
        }

        /// <summary>
        /// Method invoked when the user clicks on Ascolta button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Message message = (Message)element.DataContext;

            ShellPage.Current.AppNotification.Subtitle = "Sto scaricando il messaggio vocale che hai scelto di ascoltare. Non appena il download verrà terminato il messaggio vocale verrà aperto automaticamente.";
            ShellPage.Current.AppNotification.IsOpen = true;

            try
            {
                if (!await Launcher.LaunchFileAsync(await ClientExtensions.DownloadMessage(message)))
                {
                    ShellPage.Current.AppNotification.Subtitle = "Si è verificato un errore durante l'apertura del messaggio selezionato. Puoi provare ad aprire manualmente il file situato nella cartella \"Area personale\" nella cartella \"Download\" del tuo account.";
                }
                else
                {
                    ShellPage.Current.AppNotification.IsOpen = false;
                }
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Elimina button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarButton_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Message message = (Message)element.DataContext;

            try
            {
                await ClientExtensions.DeleteMessage(message.ID);
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }

            Page_Loaded(null, null);
        }

        /// <summary>
        /// Method invoked when the user clicks on Richiama button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Message message = (Message)element.DataContext;

            try
            {
                PhoneCallManager.ShowPhoneCallUI(message.Sender, message.Sender);
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Scrivi messaggio button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarButton_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Message message = (Message)element.DataContext;

            try
            {
                ChatMessage chat = new ChatMessage();
                chat.Recipients.Add(message.Sender);

                await ChatMessageManager.ShowComposeSmsMessageAsync(chat);
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }
    }
}
