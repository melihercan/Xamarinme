using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XamarinmeHosting
{
    public sealed class XamarinHost : IHost
    {
        public IServiceProvider Services { get; }

        internal XamarinHost(IServiceProvider services)
        {
            Services = services;
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
