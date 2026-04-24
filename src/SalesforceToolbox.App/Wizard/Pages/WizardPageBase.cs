using System.Windows.Controls;
using SalesforceToolbox.App.ViewModels;

namespace SalesforceToolbox.App.Wizard.Pages
{
    public abstract class WizardPageBase : UserControl
    {
        protected ConnectionWizardViewModel ViewModel { get; }

        protected WizardPageBase(ConnectionWizardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;
        }

        /// <summary>Returns true if the page data is valid and the wizard can advance.</summary>
        public virtual bool Validate() => true;
    }
}
