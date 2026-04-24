using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SalesforceToolbox.App.Services;
using SalesforceToolbox.Core.Interfaces;

namespace SalesforceToolbox.App.ViewModels
{
    public class PluginGalleryViewModel : INotifyPropertyChanged
    {
        private IPlugin _selectedPlugin;
        private UIElement _pluginView;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<IPlugin> Plugins { get; } = new ObservableCollection<IPlugin>();

        public IPlugin SelectedPlugin
        {
            get => _selectedPlugin;
            set
            {
                _selectedPlugin = value;
                OnPropertyChanged();
                LoadPluginView();
            }
        }

        public UIElement PluginView
        {
            get => _pluginView;
            set { _pluginView = value; OnPropertyChanged(); }
        }

        public ICommand LaunchPluginCommand { get; }

        public PluginGalleryViewModel()
        {
            LaunchPluginCommand = new RelayCommand<IPlugin>(LaunchPlugin);
            LoadPlugins();
        }

        private void LoadPlugins()
        {
            var loader = new PluginLoader();
            var plugins = loader.LoadPlugins(App.SalesforceService);
            foreach (var p in plugins)
                Plugins.Add(p);
        }

        private void LoadPluginView()
        {
            if (_selectedPlugin == null)
            {
                PluginView = null;
                return;
            }
            PluginView = _selectedPlugin.CreateView();
        }

        private void LaunchPlugin(IPlugin plugin)
        {
            if (plugin == null) return;
            SelectedPlugin = plugin;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
