using System;
using System.Windows;
using System.Windows.Media;
using SalesforceToolbox.Core.Interfaces;

namespace SalesforceToolbox.Plugins.SDK
{
    /// <summary>
    /// Base class for all SalesforceToolbox plugins.
    /// </summary>
    public abstract class PluginBase : IPlugin
    {
        protected ISalesforceService SalesforceService { get; private set; }

        public abstract string Id { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public virtual ImageSource Icon => null;

        public virtual void Initialize(ISalesforceService salesforceService)
        {
            SalesforceService = salesforceService ?? throw new ArgumentNullException(nameof(salesforceService));
            OnInitialized();
        }

        public abstract UIElement CreateView();

        /// <summary>Called after Initialize; override to perform additional setup.</summary>
        protected virtual void OnInitialized() { }
    }
}
