using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using SalesforceToolbox.App.Services;
using SalesforceToolbox.App.Views;
using SalesforceToolbox.App.Wizard;

namespace SalesforceToolbox.App.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private UIElement _currentView;
        private HamburgerMenuIconItem _selectedMenuItem;
        private bool _isConnected;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<HamburgerMenuIconItem> MenuItems { get; }
        public ObservableCollection<HamburgerMenuIconItem> OptionsMenuItems { get; }

        public HamburgerMenuIconItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                _selectedMenuItem = value;
                OnPropertyChanged();
                NavigateTo(value?.Tag as string);
            }
        }

        public UIElement CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public string ConnectionStatusText
        {
            get => _isConnected
                ? $"Connected: {App.SalesforceService.Username} ({App.SalesforceService.InstanceUrl})"
                : "Not connected";
        }

        public Visibility ConnectButtonVisibility => _isConnected ? Visibility.Collapsed : Visibility.Visible;
        public Visibility DisconnectButtonVisibility => _isConnected ? Visibility.Visible : Visibility.Collapsed;

        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand NavigateCommand { get; }

        public MainWindowViewModel()
        {
            MenuItems = new ObservableCollection<HamburgerMenuIconItem>
            {
                new HamburgerMenuIconItem
                {
                    Icon = new MahApps.Metro.IconPacks.PackIconMaterial { Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Home },
                    Label = "Home",
                    Tag = "Home"
                },
                new HamburgerMenuIconItem
                {
                    Icon = new MahApps.Metro.IconPacks.PackIconMaterial { Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.PuzzleOutline },
                    Label = "Plugins",
                    Tag = "Plugins"
                }
            };

            OptionsMenuItems = new ObservableCollection<HamburgerMenuIconItem>
            {
                new HamburgerMenuIconItem
                {
                    Icon = new MahApps.Metro.IconPacks.PackIconMaterial { Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Cog },
                    Label = "Settings",
                    Tag = "Settings"
                }
            };

            ConnectCommand = new RelayCommand(OpenConnectionWizard);
            DisconnectCommand = new RelayCommand(async () =>
            {
                await App.SalesforceService.DisconnectAsync();
                _isConnected = false;
                RefreshConnectionStatus();
            });
            NavigateCommand = new RelayCommand<HamburgerMenuIconItem>(item => NavigateTo(item?.Tag as string));

            // Navigate to Home on startup
            CurrentView = new HomeView { DataContext = new HomeViewModel() };
            SelectedMenuItem = MenuItems[0];
        }

        private void NavigateTo(string page)
        {
            switch (page)
            {
                case "Home":
                    CurrentView = new HomeView { DataContext = new HomeViewModel() };
                    break;
                case "Plugins":
                    CurrentView = new PluginGalleryView { DataContext = new PluginGalleryViewModel() };
                    break;
                case "Settings":
                    CurrentView = new SettingsView();
                    break;
            }
        }

        private void OpenConnectionWizard()
        {
            var wizard = new ConnectionWizardWindow();
            wizard.Owner = Application.Current.MainWindow;
            if (wizard.ShowDialog() == true)
            {
                _isConnected = App.SalesforceService.IsConnected;
                RefreshConnectionStatus();
            }
        }

        private void RefreshConnectionStatus()
        {
            OnPropertyChanged(nameof(ConnectionStatusText));
            OnPropertyChanged(nameof(ConnectButtonVisibility));
            OnPropertyChanged(nameof(DisconnectButtonVisibility));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
