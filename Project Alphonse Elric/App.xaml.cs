using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using Project_Alphonse_Elric.Helpers;
using Project_Alphonse_Elric.Services;
using Project_Alphonse_Elric.Views;
using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace Project_Alphonse_Elric
{
    sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            AppCenter.Start("93a2aa6e-431b-4c5b-826c-24d0073f5842", typeof(Analytics), typeof(Crashes), typeof(Push));

            InitializeComponent();

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();

            UIExtensions.SetTheme();

            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
            NavigationService.Navigate(typeof(LockedPage));
        }

        private ActivationService CreateActivationService()
        {
            UIExtensions.SetTitleBarColor();

            return new ActivationService(this, typeof(LockedPage), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return ShellPage.Current = new ShellPage();
        }
    }
}
