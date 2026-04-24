using System.Windows;
using System.Windows.Media;

namespace SalesforceToolbox.Core.Interfaces
{
    public interface IPlugin
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        ImageSource Icon { get; }
        UIElement CreateView();
        void Initialize(ISalesforceService salesforceService);
    }

    public interface IPluginMetadata
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
    }
}
