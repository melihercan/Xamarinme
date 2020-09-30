# Hosting
.NET Core have a build-in support for Hosting. A host enables developers to easily setup features such as configuration, dependency injection, logging etc. 

Xamarin don't have such support out of the box and this library aims to provide one for Xamarin apps. The library supports:
* Configuration: Uses [Xamarinme.Configuration](https://github.com/melihercan/Xamarinme/blob/master/Configuration/README.md) library that supports embedded resource configuration provider.
* Dependency Injection: Uses .NET Core dependency injection. 
* Logging: Supports Debug logging provider.
* Environment: Provides execution environment: "Development", "Production", "Staging" or a custom string. On .NET Core, the execution environment string is obtained from environment variables: "ASPNETCORE_ENVIRONMENT" and "DOTNET_ENVIRONMENT" and "Production" is the default if no environment variable is specified. For Xamarin there is no straightforward way to define environment variables. Therefore another approach has been taken. "Production" is still the default execution environment. This can be overriden by defining "XAMARIN_ENVIRONMENT" value as a root element in "appsettings.json" file. For example `"XAMARIN_ENVIRONMENT": "Development"`.

Blazor WebAssemblyHostBuilder has been used as a template to implement the library.

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
```
```cs
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
* A static `Host` variable is defined which can be used throughout the code to access the `Services` parameter to add/get services during run time.

.NET dependency injection module can inject services such as `IConfiguration`, `IXamarinHostEnvironment`, `ILogger` into the construtors of services created by dependency injection. Here is a sample service:
```cs
    public interface ISampleService
    {
        ILogger<SampleService> GetSampleLogger();
        IXamarinHostEnvironment GetXamarinHostEnvironment();
        IConfiguration GetConfiguration();
    }
```
```cs
   public class SampleService : ISampleService
    {
        private readonly ILogger<SampleService> _logger;
        private readonly IXamarinHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public SampleService(ILogger<SampleService> logger, IXamarinHostEnvironment environment, IConfiguration configuration)
        {
            _logger = logger;
            _environment = environment;
            _configuration = configuration;
        }

        public ILogger<SampleService> GetSampleLogger() => _logger;

        public IXamarinHostEnvironment GetXamarinHostEnvironment() => _environment;

        public IConfiguration GetConfiguration() => _configuration;
    }    
    
```

For other files the services can be obtained by using `App.Host`. Here are some examples to obtain built-in and user services:
```cs
            var logger = App.Host.Services.GetRequiredService<ILogger<Xxx>>();
            var environment = App.Host.Services.GetRequiredService<IXamarinHostEnvironment>();
            var configuration = App.Host.Services.GetRequiredService<IConfiguration>();
            var sampleService = App.Host.Services.GetService<ISampleService>());
```
And using services examples:
```cs
            logger.LogInformation($"Config value of Build:{configuration["Build"]}");
            logger.LogInformation($"Environment: {environemnt.Environment}");
```
## Installation
Install the NuGet packet `Xamarinme.Hosting` with VS2019 or by Package Manager Console:

`Install-Package Xamarinme.Hosting`
## API
### XamarinHostBuilder
```cs
        public XamarinHostConfiguration Configuration { get; }
        public IServiceCollection Services { get; }
        public IXamarinHostEnvironment HostEnvironment { get; }
        public ILoggingBuilder Logging { get; private set; }

        public static XamarinHostBuilder CreateDefault(EmbeddedResourceConfigurationOptions configurationOptions) { ... }
        public IHost Build() { ... }
```
### XamarinHostConfiguration
```cs
  public class XamarinHostConfiguration : IConfiguration, IConfigurationRoot, IConfigurationBuilder { ... }
```
### IXamarinHostEnvironment
```cs
    public interface IXamarinHostEnvironment
    {
        /// <summary>
        /// Gets the name of the runningenvironment. 
        /// This is configured to use the environment of the application hosting the Xamarin application.
        /// Configured to "Production" when not specified by the host.
        /// </summary>
        string Environment { get; }
    }
```







