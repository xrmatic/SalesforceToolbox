using System.Windows;
using SalesforceToolbox.Plugin.LogViewer.ViewModels;
using SalesforceToolbox.Plugin.LogViewer.Views;
using SalesforceToolbox.Plugins.SDK;

namespace SalesforceToolbox.Plugin.LogViewer
{
    public class LogViewerPlugin : SalesforcePluginBase
    {
        public override string Id => "log-viewer";
        public override string Name => "Log Viewer";
        public override string Description => "View, browse, and delete Salesforce Apex debug logs.";

        public override UIElement CreateView()
        {
            var vm = new LogViewerViewModel(SalesforceService);
            return new LogListView { DataContext = vm };
        }
    }
}
