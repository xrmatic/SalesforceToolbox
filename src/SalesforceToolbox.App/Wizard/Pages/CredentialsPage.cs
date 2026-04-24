using System.Windows;
using System.Windows.Controls;
using SalesforceToolbox.App.ViewModels;

namespace SalesforceToolbox.App.Wizard.Pages
{
    public class CredentialsPage : WizardPageBase
    {
        private TextBox _tbClientId;
        private PasswordBox _pbClientSecret;
        private TextBox _tbUsername;
        private PasswordBox _pbPassword;
        private TextBox _tbProfileName;

        public CredentialsPage(ConnectionWizardViewModel viewModel) : base(viewModel)
        {
            var panel = new StackPanel { Margin = new Thickness(40) };

            panel.Children.Add(new TextBlock
            {
                Text = "Enter Credentials",
                FontSize = 20,
                FontWeight = FontWeights.Light,
                Margin = new Thickness(0, 0, 0, 20)
            });

            AddLabeledField(panel, "Profile Name", out _tbProfileName, ViewModel.Profile.Name);
            AddLabeledField(panel, "Consumer Key (Client ID)", out _tbClientId, ViewModel.Profile.ClientId);
            AddPasswordField(panel, "Consumer Secret", out _pbClientSecret);
            AddLabeledField(panel, "Username", out _tbUsername, ViewModel.Profile.Username);
            AddPasswordField(panel, "Password + Security Token", out _pbPassword);

            panel.Children.Add(new TextBlock
            {
                Text = "Note: Append your security token to your password (e.g. MyPass123TOKEN).",
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.Gray,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 8, 0, 0)
            });

            Content = panel;
        }

        private void AddLabeledField(StackPanel panel, string label, out TextBox textBox, string defaultValue = "")
        {
            panel.Children.Add(new TextBlock { Text = label, Margin = new Thickness(0, 6, 0, 2) });
            textBox = new TextBox { Text = defaultValue ?? "", Margin = new Thickness(0, 0, 0, 4) };
            panel.Children.Add(textBox);
        }

        private void AddPasswordField(StackPanel panel, string label, out PasswordBox passwordBox)
        {
            panel.Children.Add(new TextBlock { Text = label, Margin = new Thickness(0, 6, 0, 2) });
            passwordBox = new PasswordBox { Margin = new Thickness(0, 0, 0, 4) };
            panel.Children.Add(passwordBox);
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(_tbProfileName.Text))
            {
                MessageBox.Show("Please enter a profile name.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(_tbClientId.Text))
            {
                MessageBox.Show("Please enter the Consumer Key.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(_pbClientSecret.Password))
            {
                MessageBox.Show("Please enter the Consumer Secret.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(_tbUsername.Text))
            {
                MessageBox.Show("Please enter your username.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(_pbPassword.Password))
            {
                MessageBox.Show("Please enter your password.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            ViewModel.Profile.Name = _tbProfileName.Text.Trim();
            ViewModel.Profile.ClientId = _tbClientId.Text.Trim();
            ViewModel.Profile.ClientSecret = _pbClientSecret.Password;
            ViewModel.Profile.Username = _tbUsername.Text.Trim();
            ViewModel.Password = _pbPassword.Password;

            return true;
        }
    }
}
