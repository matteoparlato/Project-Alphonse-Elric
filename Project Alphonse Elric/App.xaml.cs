using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using Microsoft.Toolkit.Uwp.Helpers;
using Project_Alphonse_Elric.Services;
using Project_Alphonse_Elric.Views;
using System;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
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

        private const string AppCenterAPIKey = "eadb65c5-c3e6-4a17-9031-b65d732a2523";

        public App()
        {
            if (!(ApplicationData.Current.LocalSettings.Values.ContainsKey("AnalyticsDisabled") || SystemInformation.IsFirstRun))
            {
                AppCenter.Start(AppCenterAPIKey, typeof(Analytics), typeof(Crashes), typeof(Push));
            }
            else
            {
                AppCenter.Start(AppCenterAPIKey, typeof(Push));
            }

            InitializeComponent();
            UnhandledException += OnAppUnhandledException;

            // Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.

            try
            {
                TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            }
            catch (Exception)
            {
                //
            }

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
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(LockedPage), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return ShellPage.Current = new ShellPage();
        }
    }
}
