# Hosting
.NET Core have a build-in support for Hosting. A host enables developers to easily setup features such as configuration, dependency injection, logging etc. 

Xamarin don't have such support out of the box and this library aims to provide .NET hosting in Xamain apps. The library supports:
* Configuration: Uses [Xamarinme.Configuration](https://github.com/melihercan/Xamarinme/blob/master/Configuration/README.md) library that supports embedded resource configuration provider.
* Dependency Injection: Uses .NET Core dependency injection. 
* Logging: Supports Debug logging provider.
* Environment: Provides execution environment: "Development", "Production", "Staging" or custom string. The execution environemnt string is obtained from environemnt variables: "ASPNETCORE_ENVIRONMENT" and "DOTNET_ENVIRONMENT". "ASPNETCORE_ENVIRONMENT" overrides "DOTNET_ENVIRONMENT" and "Production" is default if no environment variables are specified.  
Blazor WebAssemblyHostBuilder has been uses as a template to implement the library.

## Usage
```cs
    public partial class App : Application
    {
        public static IHost Host { get; private set; }

        public App()
        {
            InitializeXamarinHostBuilder();

            InitializeComponent();
            MainPage = new AppShell();
        }

        private void InitializeXamarinHostBuilder()
        {
            var hostBuilder = XamarinHostBuilder.CreateDefault(new EmbeddedResourceConfigurationOptions
            {
                Assembly = Assembly.GetExecutingAssembly(),
                Prefix = "DemoApp"
            });

            hostBuilder.Services.AddSingleton<ISampleService, SampleService>();
            ...
            
            Host = hostBuilder.Build();
        }
    }
```
* `XamarinHostBuilder` is created by calling `CreateDefault` and providing `EmbeddedResourceConfigurationOptions` configuration builder parameter. 


