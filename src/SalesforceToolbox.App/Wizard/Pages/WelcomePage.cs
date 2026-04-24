using System.Windows;
using System.Windows.Controls;
using SalesforceToolbox.App.ViewModels;

namespace SalesforceToolbox.App.Wizard.Pages
{
    public class WelcomePage : WizardPageBase
    {
        public WelcomePage(ConnectionWizardViewModel viewModel) : base(viewModel)
        {
            var panel = new StackPanel { Margin = new Thickness(40) };

            panel.Children.Add(new TextBlock
            {
                Text = "Connect to Salesforce",
                FontSize = 24,
                FontWeight = FontWeights.Light,
                Margin = new Thickness(0, 0, 0, 16)
            });

            panel.Children.Add(new TextBlock
            {
                Text = "This wizard will guide you through connecting to a Salesforce organization.",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 12)
            });

            panel.Children.Add(new TextBlock
            {
                Text = "You will need:",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 8)
            });

            foreach (var item in new[] {
                "• A Connected App with OAuth enabled",
                "• Your Connected App's Consumer Key (Client ID)",
                "• Your Connected App's Consumer Secret",
                "• Your Salesforce username and password + security token" })
            {
                panel.Children.Add(new TextBlock
                {
                    Text = item,
                    Margin = new Thickness(12, 2, 0, 2),
                    TextWrapping = TextWrapping.Wrap
                });
            }

            Content = panel;
        }
    }
}
