using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SalesforceToolbox.App.Services;
using SalesforceToolbox.App.Wizard;
using SalesforceToolbox.Core.Models;
using SalesforceToolbox.Core.Services;

namespace SalesforceToolbox.App.ViewModels
{
    public class ConnectionWizardViewModel : INotifyPropertyChanged
    {
        private ConnectionProfile _profile;
        private string _password;
        private string _statusMessage;
        private bool _isConnecting;
        private int _currentStep;

        public event PropertyChangedEventHandler PropertyChanged;

        public ConnectionProfile Profile
        {
            get => _profile;
            set { _profile = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public bool IsConnecting
        {
            get => _isConnecting;
            set { _isConnecting = value; OnPropertyChanged(); }
        }

        public int CurrentStep
        {
            get => _currentStep;
            set { _currentStep = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ConnectionProfile> SavedProfiles { get; } = new ObservableCollection<ConnectionProfile>();

        public ConnectionWizardViewModel()
        {
            Profile = new ConnectionProfile { Name = "My Org", OrgType = OrgType.Production };
            LoadSavedProfiles();
        }

        private void LoadSavedProfiles()
        {
            var store = App.CredentialStore;
            var profiles = store.LoadProfiles();
            foreach (var p in profiles)
                SavedProfiles.Add(p);
        }

        public async System.Threading.Tasks.Task<bool> ConnectAsync()
        {
            IsConnecting = true;
            StatusMessage = "Connecting to Salesforce...";
            try
            {
                var svc = App.SalesforceService as SalesforceService;
                if (svc == null)
                {
                    StatusMessage = "Internal error: service unavailable.";
                    return false;
                }

                bool result = await svc.ConnectWithCredentialsAsync(Profile, Password);
                if (result)
                {
                    StatusMessage = $"Successfully connected to {svc.InstanceUrl}";
                    App.CredentialStore.SaveProfile(Profile);
                }
                return result;
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                return false;
            }
            finally
            {
                IsConnecting = false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
