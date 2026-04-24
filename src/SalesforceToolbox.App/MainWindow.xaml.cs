using System.Windows;
using SalesforceToolbox.App.ViewModels;

namespace SalesforceToolbox.App
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
