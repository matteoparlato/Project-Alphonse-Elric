using Helpers;
using Models;
using Project_Alphonse_Elric.Dialogs;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Project_Alphonse_Elric.Views
{
    /// <summary>
    /// ServicesPage class
    /// </summary>
    public sealed partial class ServicesPage : Page
    {
        internal Profile AccountDetails { get; private set; } = ClientExtensions.AccountDetails;

        /// <summary>
        /// Parameterless constructor of ServicesPage class.
        /// </summary>
        public ServicesPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Method invoked once navigated to the page.
        /// Shows user active options and services.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await ClientExtensions.GetOptions();
                await ClientExtensions.GetServices();
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Blocco numeri a pagamento option checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AccountDetails.ActiveOptions.PaidNumbers)
                {
                    await ClientExtensions.EnablePaidNumbers();
                }
                else
                {
                    await ClientExtensions.DisablePaidNumbers();
                }
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Sblocco connessione dati oltre il traffico dati incluso nella promozione option checkbox. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click_8(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AccountDetails.ActiveOptions.UnlockLocal)
                {
                    await ClientExtensions.EnableUnlockLocal();
                }
                else
                {
                    await ClientExtensions.DisableUnlockLocal();
                }
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Sblocco consumo dei dati in Italia oltre i 50€ option checkbox. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click_10(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AccountDetails.ActiveOptions.UnlockItaly)
                {
                    await ClientExtensions.EnableUnlockItaly();
                }
                else
                {
                    await ClientExtensions.DisableUnlockItaly();
                }
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Sblocco consumo dei dati in roaming (Europa e resto del mondo) oltre i 50€ option checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click_9(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AccountDetails.ActiveOptions.UnlockRoaming)
                {
                    await ClientExtensions.EnableUnlockRoaming();
                }
                else
                {
                    await ClientExtensions.DisableUnlockRoaming();
                }
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Trasferimento numeri nascosti alla segreteria telefonica option checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click_7(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AccountDetails.ActiveServices.RedirectToVoicemailUnknown)
                {
                    await ClientExtensions.EnableRedirectToVoicemailUnknown();
                }
                else
                {
                    await ClientExtensions.DisableRedirectToVoicemailUnknown();
                }
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Blocco numeri nascosti service checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AccountDetails.ActiveServices.BlockUnknown)
                {
                    await ClientExtensions.EnableBlockUnknown();
                }
                else
                {
                    await ClientExtensions.DisableBlockUnknown();
                }
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Inoltro verso segreteria telefonica all'estero service checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AccountDetails.ActiveServices.TransferToVoicemail)
                {
                    await ClientExtensions.EnableTransferToVoicemail();
                }
                else
                {
                    await ClientExtensions.DisableTransferToVoicemail();
                }
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Protezione contro il trasferimento di chiamate service checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AccountDetails.ActiveServices.TransferProtection)
                {
                    await ClientExtensions.EnableTransferProtection();
                }
                else
                {
                    await ClientExtensions.DisableTransferProtection();
                }
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Utente non disponibile service checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AccountDetails.ActiveServices.UserNotAvailable)
                {
                    await ClientExtensions.EnableUserNotAvailable();
                }
                else
                {
                    await ClientExtensions.DisableUserNotAvailable();
                }
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Chiamate rapide service checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AccountDetails.ActiveServices.FastCalls)
                {
                    await ClientExtensions.EnableFastCalls();
                }
                else
                {
                    await ClientExtensions.DisableFastCalls();
                }
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Filtro Chiamate & SMS/MMS service checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click_6(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AccountDetails.ActiveServices.Filter)
                {
                    await ClientExtensions.EnableFilter();
                }
                else
                {
                    await ClientExtensions.DisableFilter();
                }
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
        }

        /// <summary>
        /// Method invoked when the user clicks on Personalizza chiamate rapide hyperlink.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HyperlinkButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await new FastCallsEditor().ShowAsync();
        }
    }
}
