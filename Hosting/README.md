# Hosting
.NET Core have a build-in support for Hosting. A host enables developers to easily setup features such as configuration, dependency injection, logging etc. 

Xamarin don't have such support out of the box and this library aims to provide one for Xamarin apps. The library supports:
* Configuration: Uses [Xamarinme.Configuration](https://github.com/melihercan/Xamarinme/blob/master/Configuration/README.md) library that supports embedded resource configuration provider.
* Dependency Injection: Uses .NET Core dependency injection. 
* Logging: Supports Debug logging provider.
* Environment: Provides execution environment: "Development", "Production", "Staging" or custom string. The execution environemnt string is obtained from environemnt variables: "ASPNETCORE_ENVIRONMENT" and "DOTNET_ENVIRONMENT". "ASPNETCORE_ENVIRONMENT" overrides "DOTNET_ENVIRONMENT" and "Production" is the default if no environment variables are specified.  
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
* A host builder `XamarinHostBuilder` is created by calling `CreateDefault` and providing `EmbeddedResourceConfigurationOptions` configuration builder parameter (see [Xamarinme.Configuration](https://github.com/melihercan/Xamarinme/blob/master/Configuration/README.md) for details).
* Services can be added to the created host builder. In the above example, `ISampleService` service with implementation class `SampleService` is added.
* Finally a host is created by calling the `Build` method. Note that there is no need to call the `Run` method like in .NET Core apps as running is handled by Xamarin itself.




