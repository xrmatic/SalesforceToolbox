# SalesforceToolbox

A modular WPF desktop application for Salesforce administrators. Built on .NET Framework 4.8 with a MahApps.Metro UI and a plugin architecture that lets you extend functionality without touching the core app.

---

## Features

- **Connection Wizard** – authenticate to Production, Sandbox, or custom Salesforce orgs via Username-Password OAuth flow
- **Secure Credential Storage** – profiles encrypted with Windows DPAPI, stored in `%APPDATA%\SalesforceToolbox\profiles.json`
- **Plugin System** – drop compiled plugin DLLs into the `Plugins\` folder; they are discovered automatically at startup
- **Built-in Plugins**
  - **User Management** – browse, search, filter, and export org users; toggle active/inactive
  - **Object Explorer** – list all sObjects and inspect their fields, types, and metadata
  - **Log Viewer** – view and delete Apex debug logs

---

## Solution Structure

```
SalesforceToolbox.sln
│
├── src/
│   ├── SalesforceToolbox.Core          # Models, interfaces, services (no UI dependency)
│   ├── SalesforceToolbox.Plugins.SDK   # Base classes and helpers for plugin authors
│   └── SalesforceToolbox.App           # WPF host application (MahApps.Metro)
│
└── Plugins/
    ├── UserManagement                  # Built-in User Management plugin
    ├── ObjectExplorer                  # Built-in Object Explorer plugin
    └── LogViewer                       # Built-in Log Viewer plugin
```

---

## Prerequisites

| Requirement | Version |
|---|---|
| Windows | 10 / 11 |
| .NET Framework | 4.8 |
| Visual Studio | 2019 or 2022 (with Desktop Development workload) |
| NuGet | CLI or Visual Studio restore |

---

## Getting Started

1. **Clone** this repository.
2. **Restore NuGet packages** – open the solution in Visual Studio; packages restore automatically. Or from the solution root:
   ```
   nuget restore SalesforceToolbox.sln
   ```
3. **Build** the solution (`Ctrl+Shift+B`). Plugin DLLs are copied to `SalesforceToolbox.App\bin\Debug\Plugins\` via post-build events.
4. **Run** `SalesforceToolbox.App`.
5. Click **Connect** in the toolbar to open the Connection Wizard and authenticate to your Salesforce org.

---

## NuGet Dependencies

| Package | Version | Used by |
|---|---|---|
| MahApps.Metro | 2.4.10 | App |
| MahApps.Metro.IconPacks.Material | 4.11.0 | App |
| ControlzEx | 4.4.0 | App (MahApps peer dep) |
| Newtonsoft.Json | 13.0.3 | Core, App, Plugins |
| DeveloperForce.Force | 2.1.0 | Core (Salesforce REST API client) |

---

## Writing a Plugin

1. Create a new **Class Library (.NET Framework 4.8)** project.
2. Add a reference to **SalesforceToolbox.Plugins.SDK** (or its DLL).
3. Inherit from `SalesforcePluginBase` (for Salesforce-aware plugins) or `PluginBase`:

   ```csharp
   using SalesforceToolbox.Plugins.SDK;
   using System.Windows.Controls;

   public class MyPlugin : SalesforcePluginBase
   {
       public override string Name        => "My Plugin";
       public override string Description => "Does something useful.";
       public override string Author      => "Your Name";
       public override string Version     => "1.0.0";
       public override string IconGlyph   => "\uE8A0"; // Segoe MDL2 code point

       public override UserControl CreateView() => new MyPluginView();
   }
   ```

4. Export the plugin by implementing `IPlugin` (already done via `PluginBase`).
5. Build and drop the output DLL into the application's `Plugins\` folder.
6. Restart SalesforceToolbox – your plugin appears in the Plugin Gallery.

See `src/SalesforceToolbox.Plugins.SDK/README.md` for the full SDK reference.

---

## Architecture Notes

- **`ISalesforceService`** is the single gateway to the Salesforce REST API. The concrete `SalesforceService` implementation uses [DeveloperForce.Force](https://github.com/developerforce/Force.com-Toolkit-for-NET) under the hood.
- **`CredentialStore`** encrypts connection profiles with Windows DPAPI (`ProtectedData`) — credentials never leave the machine in plain text.
- **`PluginLoader`** scans the `Plugins\` directory at startup, loads each assembly, and reflects over exported types that implement `IPlugin`.
- **Navigation** uses a simple `NavigationService` singleton backed by a `ContentControl` in `MainWindow`. ViewModels do not hold `Frame` references.

---

## License

MIT
