# Configuration
.NET Core have a built-in support to get configuration values from a JSON file `appsettings.json`. Multiple environments are easily supported with additional nested files: `appsettings.Development.json`, `appsettings.Production.json`, etc. Environment file overrides the key value if same key exists in the base file.

Xamarin do not have such support out of the box. This library is aiming to fill this shortage by implementing an embedded resource configuration provider.

.NET Core have a modular, pluggable, and extensible Configuration building block architecture that can be customized and implemented on other platforms and Xamarin is no exception. Configuration in .NET Core is performed using one or more configuration providers. Configuration providers read configuration data from key-value pairs using a variety of configuration sources. In this library, settings files, `appsettings.json` and `appsettings.{environment}.json` configuration source scheme is being used. The flow is simple:
* User creates configuration JSON file(s) in shared or native project(s).
* Mark them as embedded resource, so that they will be embedded in the final build as resources.
* The library provides `AddEmbeddedResource` configuration extension method that accepts `configuration options` and optional `environment` string (`"Production"` is default) parameters. User simply creates and builds a new `ConfigurationBuilder` object by providing `AddEmbeddedResource` to the builder.
* During run time, embedded resource configuration provider is called which in turn gets the resource files; `appsettings.json` and `appsettings.{environment}.json`, parses them and creates key-value pairs to be retrieved by the user.

## Simple example
* `appsettings.json` file on the root folder of the shared project with Default namespace: `MyApp` (you can get the default namespace from VS2019 project properties) and marked as `Embedded resource` (on VS2019, right click properties)
``` json
{
  "Build": "Development",
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}
```
* On `App.xaml.cs` file add the following:
```cs
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }

        public App()
        {
            Configuration = new ConfigurationBuilder()
                .AddEmbeddedResource(new EmbeddedResourceConfigurationOptions { Assembly = Assembly.GetExecutingAssembly(), Prefix = "MyApp" }) 
                .Build();

            InitializeComponent();
            MainPage = new MainPage();
        }
        ...
```
* Now you can access the `App.Configuration` parameter anywhere in the project to read the configuration values, for example in `MainPage`:
```cs
        public MainPage()
        {
            var c1 = App.Configuration["Build"];                        // "Development"
            var c2 = App.Configuration["Logging:IncludeScopes"];        // false
            var c3 = App.Configuration["Logging:LogLevel:Default"];     // "Debug"
            var c4 = App.Configuration["Logging:LogLevel:System"];      // "Information"
            var c5 = App.Configuration["Logging:LogLevel:Microsoft"];   // "Information"

            InitializeComponent();
        }
```
## Notes
* The library can be used either on Shared or on Platform projects, or even both with separate instances.
* `AddEmbeddedResource` needs a reference to the assembly that contains the config files as well as embedded resource prefix as configuration options. The assembly can be detected in the library itself by using reflection but it will cause additional execution time as all assemblies shall be enumerated. It is better off specified by the user. Similar story on prefix. Prefix is concatanation of assembly default name space and folder names where the config files are taking place. Although some tricks could be used to get it with reflection, again better off specifed by the user as it is a VS2019 setting. Another optional parameter, environment specifies which environment specific json file to use; "Production" is default.
* When specifying prefix, concatenate default name space and config file folders each separated with a dot as described in API section.
## Installation
Install the NuGet packet `Xamarinme.Configuration` with VS2019 or by Package Manager Console:

`Install-Package Xamarinme.Configuration`
## API
### EmbeddedResourceConfigurationOptions
```cs
    public class EmbeddedResourceConfigurationOptions
    {
        // Assembly that contains the config files.
        public Assembly Assembly { get; set; }

        // Prefix to embedded resource files.
        //  Format: <default namespace>.<nested folders that contain config files each separated with dots>
        //  Examples:
        //          default namespace: "MyApp",     config files are on root folder         => Prefix = "MyApp"
        //          default namespace: "MyApp",     config files are on "res" folder        => Prefix = "MyApp.res"
        //          default namespace: "MyApp",     config files are on "res/x/y" folder    => Prefix = "MyApp.res.x.y"
        //          default namespace: "MyApp.iOS", config files are on "res/x/y" folder    => Prefix = "MyApp.iOS.res.x.y"
        public string Prefix { get; set; }
    }
```
### EmbeddedResourceConfigurationSource
```cs
    public class EmbeddedResourceConfigurationSource : IConfigurationSource
    {
        public EmbeddedResourceConfigurationOptions Options { get; set; }
        public string Environment { get; set; } = "Production";

        public IConfigurationProvider Build(IConfigurationBuilder builder) => 
            new EmbeddedResourceConfigurationProvider(Options, Environment);
    }
```
### AddEmbeddedResource(options, environment) 
Parameter | Type | Description
--- | --- | ---
options | EmbeddedResourceConfigurationOptions | Options for embedded resource configuration.
environment | string | App running environment: "Development", Staging", "Production" or custom defined string
### AddEmbeddedResource(configurationSource) 
Parameter | Type | Description
--- | --- | ---
configurationSource | Action | Callback to action with `EmbeddedResourceConfigurationSource` as parameter.

## Usage
* Add usings 
```cs
using Microsoft.Extensions.Configuration;
using Xamarinme;
```
* Add `AddEmbeddedResource` into the `ConfigurationBuilder` object.







