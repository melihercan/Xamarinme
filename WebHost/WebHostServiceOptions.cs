using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace XamarinmeWebHost
{
    internal class WebHostServiceOptions
    {
        public Action<HostBuilderContext, IApplicationBuilder> ConfigureApp { get; internal set; }
    }
}