using System.Windows;
using MahApps.Metro.Controls;
using SalesforceToolbox.App.ViewModels;
using SalesforceToolbox.App.Wizard.Pages;

namespace SalesforceToolbox.App.Wizard
{
    public partial class ConnectionWizardWindow : MetroWindow
    {
        private readonly ConnectionWizardViewModel _viewModel;
        private readonly WizardPageBase[] _pages;
        private int _currentPageIndex;

        public ConnectionWizardWindow()
        {
            InitializeComponent();
            _viewModel = new ConnectionWizardViewModel();

            _pages = new WizardPageBase[]
            {
                new WelcomePage(_viewModel),
                new OrgTypePage(_viewModel),
                new CredentialsPage(_viewModel),
                new AuthPage(_viewModel),
                new SuccessPage(_viewModel)
            };

            ShowPage(0);
        }

        private void ShowPage(int index)
        {
            _currentPageIndex = index;
            PageContent.Content = _pages[index];
            StepIndicator.Text = $"Step {index + 1} of {_pages.Length}";

            BackButton.IsEnabled = index > 0 && index < _pages.Length - 1;
            NextButton.Content = index == _pages.Length - 2 ? "Connect" : "Next";
            NextButton.IsEnabled = index < _pages.Length - 1;
            CancelButton.Content = index == _pages.Length - 1 ? "Close" : "Cancel";
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_pages[_currentPageIndex].Validate())
                return;

            if (_currentPageIndex == 2) // Credentials page -> Auth page
            {
                ShowPage(3); // AuthPage
                bool success = await _viewModel.ConnectAsync();
                if (success)
                    ShowPage(4); // SuccessPage
                else
                    ShowPage(2); // Back to credentials with error
            }
            else if (_currentPageIndex < _pages.Length - 1)
            {
                ShowPage(_currentPageIndex + 1);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPageIndex > 0)
                ShowPage(_currentPageIndex - 1);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPageIndex == _pages.Length - 1)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                DialogResult = false;
                Close();
            }
        }
    }
}
