using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SalesforceToolbox.App.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string WelcomeMessage => "Welcome to Salesforce Toolbox";
        public string SubMessage => "Connect to your Salesforce org to get started, then explore plugins from the Plugins menu.";

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
