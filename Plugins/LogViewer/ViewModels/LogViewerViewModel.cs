using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SalesforceToolbox.Core.Interfaces;
using SalesforceToolbox.Core.Models;

namespace SalesforceToolbox.Plugin.LogViewer.ViewModels
{
    public class LogViewerViewModel : INotifyPropertyChanged
    {
        private readonly ISalesforceService _salesforceService;
        private ObservableCollection<ApexLog> _logs;
        private ApexLog _selectedLog;
        private string _logContent;
        private bool _isLoading;
        private string _statusMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ApexLog> Logs
        {
            get => _logs;
            set { _logs = value; OnPropertyChanged(); }
        }

        public ApexLog SelectedLog
        {
            get => _selectedLog;
            set
            {
                _selectedLog = value;
                OnPropertyChanged();
                if (value != null)
                    _ = LoadLogBodyAsync(value.Id);
                else
                    LogContent = string.Empty;
            }
        }

        public string LogContent
        {
            get => _logContent;
            set { _logContent = value; OnPropertyChanged(); }
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
        public ICommand DeleteLogCommand { get; }
        public ICommand DeleteAllLogsCommand { get; }

        public LogViewerViewModel(ISalesforceService salesforceService)
        {
            _salesforceService = salesforceService;
            _logs = new ObservableCollection<ApexLog>();

            RefreshCommand = new RelayCommand(async () => await LoadLogsAsync());
            DeleteLogCommand = new RelayCommand<ApexLog>(async log => await DeleteLogAsync(log));
            DeleteAllLogsCommand = new RelayCommand(async () => await DeleteAllLogsAsync());

            _ = LoadLogsAsync();
        }

        private async Task LoadLogsAsync()
        {
            IsLoading = true;
            LogContent = string.Empty;
            StatusMessage = "Loading logs...";
            try
            {
                var logs = await _salesforceService.GetApexLogsAsync();
                Logs = new ObservableCollection<ApexLog>(logs);
                StatusMessage = $"{Logs.Count} logs loaded.";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error loading logs: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadLogBodyAsync(string logId)
        {
            IsLoading = true;
            LogContent = "Loading log content...";
            try
            {
                LogContent = await _salesforceService.GetApexLogBodyAsync(logId);
            }
            catch (System.Exception ex)
            {
                LogContent = $"Error loading log: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteLogAsync(ApexLog log)
        {
            if (log == null) return;
            var result = MessageBox.Show($"Delete log for {log.LogUser?.Name} ({log.LogLength} bytes)?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            IsLoading = true;
            try
            {
                await _salesforceService.DeleteApexLogAsync(log.Id);
                Logs.Remove(log);
                if (SelectedLog == log) SelectedLog = null;
                StatusMessage = "Log deleted.";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error deleting log: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteAllLogsAsync()
        {
            var result = MessageBox.Show($"Delete all {Logs.Count} logs? This cannot be undone.",
                "Confirm Delete All", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            IsLoading = true;
            StatusMessage = "Deleting all logs...";
            int deleted = 0;
            try
            {
                var toDelete = new System.Collections.Generic.List<ApexLog>(Logs);
                foreach (var log in toDelete)
                {
                    await _salesforceService.DeleteApexLogAsync(log.Id);
                    Logs.Remove(log);
                    deleted++;
                }
                SelectedLog = null;
                StatusMessage = $"Deleted {deleted} logs.";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error: {ex.Message} ({deleted} deleted so far)";
            }
            finally
            {
                IsLoading = false;
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
