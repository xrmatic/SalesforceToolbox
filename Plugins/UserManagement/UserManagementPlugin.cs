using System.Windows;
using SalesforceToolbox.Core.Interfaces;
using SalesforceToolbox.Plugin.UserManagement.ViewModels;
using SalesforceToolbox.Plugin.UserManagement.Views;
using SalesforceToolbox.Plugins.SDK;

namespace SalesforceToolbox.Plugin.UserManagement
{
    public class UserManagementPlugin : SalesforcePluginBase
    {
        public override string Id => "user-management";
        public override string Name => "User Management";
        public override string Description => "View, search, and manage Salesforce users.";

        public override UIElement CreateView()
        {
            var vm = new UserListViewModel(SalesforceService);
            return new UserListView { DataContext = vm };
        }
    }
}
