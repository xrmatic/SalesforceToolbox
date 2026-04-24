using System.Windows;
using SalesforceToolbox.Plugin.ObjectExplorer.ViewModels;
using SalesforceToolbox.Plugin.ObjectExplorer.Views;
using SalesforceToolbox.Plugins.SDK;

namespace SalesforceToolbox.Plugin.ObjectExplorer
{
    public class ObjectExplorerPlugin : SalesforcePluginBase
    {
        public override string Id => "object-explorer";
        public override string Name => "Object Explorer";
        public override string Description => "Browse Salesforce objects and their fields.";

        public override UIElement CreateView()
        {
            var vm = new ObjectExplorerViewModel(SalesforceService);
            return new ObjectListView { DataContext = vm };
        }
    }
}
