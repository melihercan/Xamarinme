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
        public static IConfiguration Configuration { get; private set; }

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
* Now you can access the `App.Configuration` parameter anywhere in the project to read the configuration values, for example in `MainPage`:
```cs
        public MainPage()
        {
            var c1 = App.Configuration["Environment"];                  // "Development"
            var c2 = App.Configuration["Logging:IncludeScopes"];        // false
            var c3 = App.Configuration["Logging:LogLevel:Default"];     // "Debug"
            var c4 = App.Configuration["Logging:LogLevel:System"];      // "Information"
            var c5 = App.Configuration["Logging:LogLevel:Microsoft"];   // "Information"

            InitializeComponent();
        }
```

## Points to Consider
* The library can be used either on Shared or on Platform projects, or even both with separate instances.
* Currently only `JSON` format is supported. Planning to add `XML` soon.
* `AddEmbeddedResource` needs reference to the assembly that contains the config files as well as default namespace beside the obvious parameter of file list. The assembly can be detected in the library itself by using reflection but it can be tricky as we are supporting both shared and platform specific projects. It is better off specified by the user. Similar story on default namespace. Although some tricks could be used to get it with reflection, again better off specifed by the user as it is a VS2019 setting.
* When specifying file list, add the folders to the file names if the file is not at the root. For example:
`resource/appsettings.json`, `x/y/z/appsettings.json`.
* On a multiple files cases, the previous setting will be overwritten if the current file also specifes the same key. Hence the order of file list is important.
* Environment files should be specified manually in the file list as Configuration module has no idea what environment we are running on. 







