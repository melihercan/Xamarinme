# Configuration
.NET Core have a built-in support to get configuration values from a JSON file (`appsettings.json` by default). Multiple environments are easily supported with additional nested files: `appsettings.Development.json`, `appsettings.Production.json`, etc.

Xamarin do not have such support out of the box. This library is aiming to fill this shortage by implementing an embedded resource configuration provider.

.NET Core have a modular, pluggable, and extensible Configuration building block architecture that can be customized and implemented on other platforms and Xamarin is no exception. Configuration in .NET Core is performed using one or more configuration providers. Configuration providers read configuration data from key-value pairs using a variety of configuration sources. In this library, settings files, such as `appsettings.json` configuration source scheme is being used. The mechanism is simple:
* User creates configuration JSON file(s) in shared or native project(s).
* Mark them as embedded resource, so that they will be embedded in the final build as resources.
* The library provides `AddEmbeddedResource` configuration extension that accepts `assembly`, `defaultNamespace` and `fileNames` parameters. User simply creates and builds a new `ConfigurationBuilder` object by providing `AddEmbeddedResource` as a parameter.
* During run time, embedded resource configuration provider is called which in turn gets the listed resource files, parses them and creates key-value pairs to be retrieved by the user.

## Simple example
* `appsettings.json` file on the root folder of the shared project with Default namespace: `MyApp` (you can get the default namespace from VS2019 project properties) and marked as `Embedded resource` (on VS2019, right click properties)
``` json
{
  "Environment": "Development",
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
        public static IConfiguration Configuration { get; private set}

        public App()
        {
            Configuration = new ConfigurationBuilder()
                .AddEmbeddedResource(Assembly.GetExecutingAssembly(), "MyApp", new string[] { "appsettings.json" })
                .Build();

            InitializeComponent();
            MainPage = new MainPage();
        }
        ...
```







