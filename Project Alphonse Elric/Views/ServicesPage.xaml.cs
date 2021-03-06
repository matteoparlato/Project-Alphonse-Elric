﻿using Helpers;
using Project_Alphonse_Elric.Core.Helpers;
using Project_Alphonse_Elric.Core.Models;
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
        internal Profile AccountDetails { get; private set; } = Singleton<ClientExtensions>.Instance.AccountDetails;

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
                await Singleton<ClientExtensions>.Instance.GetOptions();
                await Singleton<ClientExtensions>.Instance.GetServices();
            }
            catch (Exception ex) { ShellPage.Current.HandleExceptionNotification(ex); }
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
                    await Singleton<ClientExtensions>.Instance.SendEnableRequest((string)checkBox.CommandParameter);
                }
                else
                {
                    await Singleton<ClientExtensions>.Instance.SendDisableRequest((string)checkBox.CommandParameter);
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
            await new FastCallsEditorDialog().ShowAsync();
        }
    }
}
