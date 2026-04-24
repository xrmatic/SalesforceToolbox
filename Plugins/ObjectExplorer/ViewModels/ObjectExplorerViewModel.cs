using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using SalesforceToolbox.Core.Interfaces;
using SalesforceToolbox.Core.Models;

namespace SalesforceToolbox.Plugin.ObjectExplorer.ViewModels
{
    public class ObjectExplorerViewModel : INotifyPropertyChanged
    {
        private readonly ISalesforceService _salesforceService;
        private ObservableCollection<SObjectInfo> _objects;
        private ObservableCollection<SObjectInfo> _filteredObjects;
        private SObjectInfo _selectedObject;
        private SObjectDetail _selectedObjectDetail;
        private string _searchText;
        private bool _isLoading;
        private string _statusMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<SObjectInfo> FilteredObjects
        {
            get => _filteredObjects;
            set { _filteredObjects = value; OnPropertyChanged(); }
        }

        public SObjectInfo SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                OnPropertyChanged();
                if (value != null)
                    _ = LoadObjectDetailAsync(value.Name);
            }
        }

        public SObjectDetail SelectedObjectDetail
        {
            get => _selectedObjectDetail;
            set { _selectedObjectDetail = value; OnPropertyChanged(); }
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

        public ObjectExplorerViewModel(ISalesforceService salesforceService)
        {
            _salesforceService = salesforceService;
            _objects = new ObservableCollection<SObjectInfo>();
            _filteredObjects = new ObservableCollection<SObjectInfo>();
            RefreshCommand = new RelayCommand(async () => await LoadObjectsAsync());
            _ = LoadObjectsAsync();
        }

        private async Task LoadObjectsAsync()
        {
            IsLoading = true;
            StatusMessage = "Loading objects...";
            try
            {
                var objects = await _salesforceService.GetSObjectsAsync();
                _objects = new ObservableCollection<SObjectInfo>(objects.OrderBy(o => o.Label));
                ApplyFilter();
                StatusMessage = $"{_objects.Count} objects loaded.";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadObjectDetailAsync(string objectName)
        {
            IsLoading = true;
            try
            {
                SelectedObjectDetail = await _salesforceService.DescribeObjectAsync(objectName);
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error loading detail: {ex.Message}";
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
                FilteredObjects = new ObservableCollection<SObjectInfo>(_objects);
                return;
            }
            var filter = _searchText.ToLowerInvariant();
            FilteredObjects = new ObservableCollection<SObjectInfo>(_objects.Where(o =>
                (o.Label?.ToLowerInvariant().Contains(filter) ?? false) ||
                (o.Name?.ToLowerInvariant().Contains(filter) ?? false)));
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
}
