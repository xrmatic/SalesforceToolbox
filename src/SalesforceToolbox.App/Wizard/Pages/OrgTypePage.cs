using System.Windows;
using System.Windows.Controls;
using SalesforceToolbox.App.ViewModels;
using SalesforceToolbox.Core.Models;

namespace SalesforceToolbox.App.Wizard.Pages
{
    public class OrgTypePage : WizardPageBase
    {
        private RadioButton _rbProduction;
        private RadioButton _rbSandbox;
        private RadioButton _rbCustom;
        private TextBox _tbCustomUrl;

        public OrgTypePage(ConnectionWizardViewModel viewModel) : base(viewModel)
        {
            var panel = new StackPanel { Margin = new Thickness(40) };

            panel.Children.Add(new TextBlock
            {
                Text = "Select Organization Type",
                FontSize = 20,
                FontWeight = FontWeights.Light,
                Margin = new Thickness(0, 0, 0, 20)
            });

            _rbProduction = new RadioButton
            {
                Content = "Production (login.salesforce.com)",
                IsChecked = true,
                Margin = new Thickness(0, 0, 0, 8)
            };
            _rbSandbox = new RadioButton
            {
                Content = "Sandbox (test.salesforce.com)",
                Margin = new Thickness(0, 0, 0, 8)
            };
            _rbCustom = new RadioButton
            {
                Content = "Custom Domain",
                Margin = new Thickness(0, 0, 0, 8)
            };

            _tbCustomUrl = new TextBox
            {
                Text = "https://",
                IsEnabled = false,
                Margin = new Thickness(20, 0, 0, 0)
            };

            _rbCustom.Checked += (s, e) => _tbCustomUrl.IsEnabled = true;
            _rbProduction.Checked += (s, e) => _tbCustomUrl.IsEnabled = false;
            _rbSandbox.Checked += (s, e) => _tbCustomUrl.IsEnabled = false;

            panel.Children.Add(_rbProduction);
            panel.Children.Add(_rbSandbox);
            panel.Children.Add(_rbCustom);
            panel.Children.Add(_tbCustomUrl);
            Content = panel;
        }

        public override bool Validate()
        {
            if (_rbProduction.IsChecked == true)
            {
                ViewModel.Profile.OrgType = OrgType.Production;
                ViewModel.Profile.InstanceUrl = "https://login.salesforce.com";
            }
            else if (_rbSandbox.IsChecked == true)
            {
                ViewModel.Profile.OrgType = OrgType.Sandbox;
                ViewModel.Profile.InstanceUrl = "https://test.salesforce.com";
            }
            else
            {
                string url = _tbCustomUrl.Text?.Trim();
                if (string.IsNullOrEmpty(url) || !url.StartsWith("https://"))
                {
                    MessageBox.Show("Please enter a valid custom domain URL starting with https://",
                        "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                ViewModel.Profile.OrgType = OrgType.Custom;
                ViewModel.Profile.InstanceUrl = url;
            }
            return true;
        }
    }
}
