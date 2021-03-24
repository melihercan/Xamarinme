﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace XamarinmeWebHost
{
    // We can't reference real servers in this sample without creating a circular repo dependency.
    // This fake server lets us at least run the code.
    internal class EmbedIoServer : IServer
    {
        public IFeatureCollection Features => new FeatureCollection();

        public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public void Dispose()
        {
        }
    }

    public static class FakeServerWebHostBuilderExtensions
    {
        public static IHostBuilder UseEmbedIoServer(this IHostBuilder builder)
        {
            return builder.ConfigureServices((builderContext, services) => services.AddSingleton<IServer, EmbedIoServer>());
        }
    }
}
