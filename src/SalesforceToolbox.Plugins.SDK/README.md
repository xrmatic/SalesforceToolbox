# SalesforceToolbox Plugin SDK

## Overview
The Plugin SDK provides base classes for building SalesforceToolbox plugins.

## Quick Start

1. Create a new Class Library (.NET 4.8) project.
2. Add a reference to `SalesforceToolbox.Plugins.SDK.dll` and `SalesforceToolbox.Core.dll`.
3. Inherit from `SalesforcePluginBase` (or `PluginBase` for non-Salesforce plugins).
4. Implement the required abstract members.
5. Place the compiled DLL in the `Plugins` folder next to the application.

## Example

```csharp
public class MyPlugin : SalesforcePluginBase
{
    public override string Id => "my-plugin";
    public override string Name => "My Plugin";
    public override string Description => "Does awesome things.";

    public override UIElement CreateView()
    {
        return new MyPluginView { DataContext = new MyViewModel(SalesforceService) };
    }
}
```

## Plugin Discovery
Plugins are discovered at runtime by scanning the `Plugins` folder for DLLs
that contain types implementing `IPlugin`.
