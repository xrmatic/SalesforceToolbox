using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SalesforceToolbox.Core.Interfaces;
using SalesforceToolbox.Core.Models;

namespace SalesforceToolbox.Plugin.UserManagement.ViewModels
{
    public class UserListViewModel : INotifyPropertyChanged
    {
        private readonly ISalesforceService _salesforceService;
        private ObservableCollection<UserInfo> _users;
        private ObservableCollection<UserInfo> _filteredUsers;
        private UserInfo _selectedUser;
        private string _searchText;
        private bool _isLoading;
        private string _statusMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<UserInfo> FilteredUsers
        {
            get => _filteredUsers;
            set { _filteredUsers = value; OnPropertyChanged(); }
        }

        public UserInfo SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ICommand RefreshCommand { get; }
        public ICommand ExportCsvCommand { get; }
        public ICommand ToggleActiveCommand { get; }

        public UserListViewModel(ISalesforceService salesforceService)
        {
            _salesforceService = salesforceService;
            _users = new ObservableCollection<UserInfo>();
            _filteredUsers = new ObservableCollection<UserInfo>();

            RefreshCommand = new RelayCommand(async () => await LoadUsersAsync());
            ExportCsvCommand = new RelayCommand(ExportToCsv);
            ToggleActiveCommand = new RelayCommand<UserInfo>(async user => await ToggleActiveAsync(user));

            _ = LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            IsLoading = true;
            StatusMessage = "Loading users...";
            try
            {
                var users = await _salesforceService.GetUsersAsync(SearchText);
                _users = new ObservableCollection<UserInfo>(users);
                ApplyFilter();
                StatusMessage = $"{_users.Count} users loaded.";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error loading users: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(_searchText))
            {
                FilteredUsers = new ObservableCollection<UserInfo>(_users);
                return;
            }

            var filter = _searchText.ToLowerInvariant();
            FilteredUsers = new ObservableCollection<UserInfo>(_users.Where(u =>
                (u.Name?.ToLowerInvariant().Contains(filter) ?? false) ||
                (u.Username?.ToLowerInvariant().Contains(filter) ?? false) ||
                (u.Email?.ToLowerInvariant().Contains(filter) ?? false)));
        }

        private async Task ToggleActiveAsync(UserInfo user)
        {
            if (user == null) return;
            try
            {
                IsLoading = true;
                bool newStatus = !user.IsActive;
                await _salesforceService.QueryAsync<object>(
                    $"SELECT Id FROM User WHERE Id = '{user.Id}'");
                // In a real implementation we'd call PATCH via the REST API
                MessageBox.Show($"Toggle active for {user.Name} requires update API call (not yet implemented).",
                    "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ExportToCsv()
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    FileName = "salesforce_users.csv"
                };
                if (dialog.ShowDialog() != true) return;

                var sb = new StringBuilder();
                sb.AppendLine("Id,Name,Username,Email,IsActive,Profile,UserType,LastLoginDate,CreatedDate");
                foreach (var user in FilteredUsers)
                {
                    sb.AppendLine($"\"{user.Id}\",\"{user.Name}\",\"{user.Username}\",\"{user.Email}\"," +
                                  $"{user.IsActive},\"{user.Profile?.Name}\",\"{user.UserType}\"," +
                                  $"\"{user.LastLoginDate}\",\"{user.CreatedDate}\"");
                }
                File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show($"Exported {FilteredUsers.Count} users to {dialog.FileName}",
                    "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    internal class RelayCommand : ICommand
    {
        private readonly System.Action _execute;
        public RelayCommand(System.Action execute) { _execute = execute; }
        public event System.EventHandler CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object p) => true;
        public void Execute(object p) => _execute();
    }

    internal class RelayCommand<T> : ICommand
    {
        private readonly System.Action<T> _execute;
        public RelayCommand(System.Action<T> execute) { _execute = execute; }
        public event System.EventHandler CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object p) => true;
        public void Execute(object p) => _execute((T)p);
    }
}
