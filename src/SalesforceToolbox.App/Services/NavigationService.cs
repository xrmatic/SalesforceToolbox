using System;
using System.Windows;
using System.Windows.Controls;

namespace SalesforceToolbox.App.Services
{
    public class NavigationService
    {
        private static NavigationService _instance;
        public static NavigationService Instance => _instance ??= new NavigationService();

        private ContentControl _frame;

        public void Register(ContentControl frame)
        {
            _frame = frame;
        }

        public void NavigateTo(UIElement view)
        {
            if (_frame == null)
                throw new InvalidOperationException("NavigationService not registered.");
            _frame.Content = view;
        }
    }
}
