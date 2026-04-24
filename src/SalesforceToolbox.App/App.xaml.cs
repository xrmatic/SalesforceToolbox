using System;
using System.Windows;
using SalesforceToolbox.Core.Services;

namespace SalesforceToolbox.App
{
    public partial class App : Application
    {
        public static SalesforceService SalesforceService { get; private set; }
        public static CredentialStore CredentialStore { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            CredentialStore = new CredentialStore();
            SalesforceService = new SalesforceService();

            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show(
                    $"An unexpected error occurred:\n\n{args.Exception.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                args.Handled = true;
            };
        }
    }
}
