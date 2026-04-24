using System.Windows;

namespace SalesforceToolbox.Plugins.SDK.Template
{
    /// <summary>
    /// Sample plugin demonstrating how to create a SalesforceToolbox plugin.
    /// Copy this file to your plugin project and modify as needed.
    /// </summary>
    public class SamplePlugin : SalesforcePluginBase
    {
        public override string Id => "sample-plugin";
        public override string Name => "Sample Plugin";
        public override string Description => "A sample plugin showing the plugin structure.";

        public override UIElement CreateView()
        {
            return new SamplePluginView();
        }
    }
}
