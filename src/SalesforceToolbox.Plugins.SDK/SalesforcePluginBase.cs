using System.Windows;
using SalesforceToolbox.Core.Interfaces;

namespace SalesforceToolbox.Plugins.SDK
{
    /// <summary>
    /// Extended base class for plugins that need Salesforce connectivity checks.
    /// </summary>
    public abstract class SalesforcePluginBase : PluginBase
    {
        protected bool EnsureConnected()
        {
            if (SalesforceService == null || !SalesforceService.IsConnected)
            {
                MessageBox.Show(
                    "Not connected to Salesforce. Please connect before using this plugin.",
                    "Not Connected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}
