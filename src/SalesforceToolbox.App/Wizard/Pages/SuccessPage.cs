using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SalesforceToolbox.App.ViewModels;

namespace SalesforceToolbox.App.Wizard.Pages
{
    public class SuccessPage : WizardPageBase
    {
        public SuccessPage(ConnectionWizardViewModel viewModel) : base(viewModel)
        {
            var panel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(40)
            };

            panel.Children.Add(new TextBlock
            {
                Text = "✓",
                FontSize = 48,
                Foreground = Brushes.Green,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 12)
            });

            panel.Children.Add(new TextBlock
            {
                Text = "Connected Successfully!",
                FontSize = 22,
                FontWeight = FontWeights.Light,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 12)
            });

            var statusText = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 400,
                TextAlignment = TextAlignment.Center,
                Foreground = Brushes.Gray
            };
            statusText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("StatusMessage"));
            panel.Children.Add(statusText);

            panel.Children.Add(new TextBlock
            {
                Text = "Click Close to start using Salesforce Toolbox.",
                Margin = new Thickness(0, 16, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.Gray
            });

            Content = panel;
        }
    }
}
