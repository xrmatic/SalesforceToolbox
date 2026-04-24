using System.Windows;
using System.Windows.Controls;
using SalesforceToolbox.App.ViewModels;

namespace SalesforceToolbox.App.Wizard.Pages
{
    public class AuthPage : WizardPageBase
    {
        public AuthPage(ConnectionWizardViewModel viewModel) : base(viewModel)
        {
            var panel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(40)
            };

            panel.Children.Add(new TextBlock
            {
                Text = "Connecting...",
                FontSize = 20,
                FontWeight = FontWeights.Light,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 16)
            });

            var progress = new ProgressBar
            {
                IsIndeterminate = true,
                Height = 8,
                Width = 300,
                Margin = new Thickness(0, 0, 0, 16)
            };
            panel.Children.Add(progress);

            var statusText = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 400
            };
            statusText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("StatusMessage"));
            panel.Children.Add(statusText);

            Content = panel;
        }
    }
}
