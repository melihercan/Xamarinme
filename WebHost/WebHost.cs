using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using XamarinmeWebHost;
using HarmonyLib;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.Extensions.Primitives;

namespace Xamarinme
{
    // aspnetcore v2.2.7 = commit be0a4a7f4c
    // runtime v5.0 = commit cf258a14b70

    public class WebHost
    {
#if true
        public class Startup
        {
            private static readonly byte[] _helloWorldBytes = Encoding.UTF8.GetBytes("Hello Xamarin, greetings from Kestrel");

            public void Configure(IApplicationBuilder app)
            {
                app.Run((httpContext) =>
                {
                    var response = httpContext.Response;
                    response.StatusCode = 200;
                    response.ContentType = "text/plain";

                    var helloWorld = _helloWorldBytes;
                    response.ContentLength = helloWorld.Length;
                    try
                    {
                        return response.Body.WriteAsync(helloWorld, 0, helloWorld.Length);
                    }
                    catch(Exception ex)
                    {
                        var x = ex.Message;
                        return Task.CompletedTask;
                    }
                });
            }
        }

        public static Task Main(string[] args)
        {
            var harmony = new Harmony("org.melihercan.Xamarinme.WebHost");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            var webHost = new WebHostBuilder()
                .ConfigureAppConfiguration((config) =>
                {
                    config.AddEmbeddedResource(
                        new EmbeddedResourceConfigurationOptions
                        {
                            Assembly = Assembly.GetExecutingAssembly(),
                            Prefix = "XamarinmeWebHost"
                        });

                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IHostLifetime, ConsoleLifetimeEx>();
                })
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 5000);
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            return webHost.RunExAsync();
        }
#endif


#if false
        public static async Task Main(string[] args)
        {
            //var host = new HostBuilder()
            //  .ConfigureHostConfiguration((config) =>
            //  {
            //      config.AddEmbeddedResource(
            //          new EmbeddedResourceConfigurationOptions
            //          {
            //              Assembly = Assembly.GetExecutingAssembly(),
            //              Prefix = "XamarinmeWebHost"
            //          });
            //    })
            //    .ConfigureAppConfiguration((hostContext, config) =>
            //    {
            //    })
            //    .ConfigureServices((hostContext, services) =>
            //    {
            //      // Override the default host lifetime with ours.
            //      services.AddSingleton<IHostLifetime, ConsoleLifetimeEx>();
            //    })
            //    .UseEmbedIoServer()
            //    .ConfigureWebHost((hostContext, app) =>
            //    {
            //        app.Run(async (context) =>
            //        {
            //            await context.Response.WriteAsync("Hello World!");
            //        });
            //    })
            //    .Build();
            //
            //System.Diagnostics.Debug.WriteLine($"-------- Running WebHost");
            //await host.RunAsync();


#if true
            var hostBuilder = new HostBuilder();

            hostBuilder.ConfigureHostConfiguration((config) =>
            {
                config.AddEmbeddedResource(
                    new EmbeddedResourceConfigurationOptions
                    {
                        Assembly = Assembly.GetExecutingAssembly(),
                        Prefix = "XamarinmeWebHost"
                    });
            });

            hostBuilder.ConfigureAppConfiguration((hostContext, config) =>
            {
            });

            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                // Override the default host lifetime with ours.
                services.AddSingleton<IHostLifetime, ConsoleLifetimeEx>();
            });

            hostBuilder.UseEmbedIoServer();

            hostBuilder.ConfigureWebHost((hostContext, app) =>
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            var host = hostBuilder.Build();

            System.Diagnostics.Debug.WriteLine($"-------- Running WebHost");
            await host.RunAsync();
#endif



        }
#endif
    }

    [HarmonyPatch(typeof(HeaderUtilities), "FormatDate", new Type[] { typeof(DateTimeOffset), typeof(bool) })]
    class FormatDatePatch
    {
        [HarmonyPrefix]
        static bool FormatDate(ref string __result, DateTimeOffset dateTime, bool quoted)
        {
            __result = DateTimeFormatterPatch.ToRfc1123String(dateTime, quoted);
            return false;
        }
    }

    internal static class DateTimeFormatterPatch
    {
        private static readonly DateTimeFormatInfo FormatInfo = CultureInfo.InvariantCulture.DateTimeFormat;

        private static readonly string[] MonthNames = FormatInfo.AbbreviatedMonthNames;
        private static readonly string[] DayNames = FormatInfo.AbbreviatedDayNames;

        private static readonly int Rfc1123DateLength = "ddd, dd MMM yyyy HH:mm:ss GMT".Length;
        private static readonly int QuotedRfc1123DateLength = Rfc1123DateLength + 2;

        // ASCII numbers are in the range 48 - 57.
        private const int AsciiNumberOffset = 0x30;

        private const string Gmt = "GMT";
        private const char Comma = ',';
        private const char Space = ' ';
        private const char Colon = ':';
        private const char Quote = '"';

        public static string ToRfc1123String(this DateTimeOffset dateTime)
        {
            return ToRfc1123String(dateTime, false);
        }

        public static string ToRfc1123String(this DateTimeOffset dateTime, bool quoted)
        {
            var universal = dateTime.UtcDateTime;

            var length = quoted ? QuotedRfc1123DateLength : Rfc1123DateLength;
            var target = new InplaceStringBuilderPatch(length);

            if (quoted)
            {
                target.Append(Quote);
            }

            target.Append(DayNames[(int)universal.DayOfWeek]);
            target.Append(Comma);
            target.Append(Space);
            AppendNumber(ref target, universal.Day);
            target.Append(Space);
            target.Append(MonthNames[universal.Month - 1]);
            target.Append(Space);
            AppendYear(ref target, universal.Year);
            target.Append(Space);
            AppendTimeOfDay(ref target, universal.TimeOfDay);
            target.Append(Space);
            target.Append(Gmt);

            if (quoted)
            {
                target.Append(Quote);
            }

            return target.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AppendYear(ref InplaceStringBuilderPatch target, int year)
        {
            target.Append(GetAsciiChar(year / 1000));
            target.Append(GetAsciiChar(year % 1000 / 100));
            target.Append(GetAsciiChar(year % 100 / 10));
            target.Append(GetAsciiChar(year % 10));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AppendTimeOfDay(ref InplaceStringBuilderPatch target, TimeSpan timeOfDay)
        {
            AppendNumber(ref target, timeOfDay.Hours);
            target.Append(Colon);
            AppendNumber(ref target, timeOfDay.Minutes);
            target.Append(Colon);
            AppendNumber(ref target, timeOfDay.Seconds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AppendNumber(ref InplaceStringBuilderPatch target, int number)
        {
            target.Append(GetAsciiChar(number / 10));
            target.Append(GetAsciiChar(number % 10));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char GetAsciiChar(int value)
        {
            return (char)(AsciiNumberOffset + value);
        }
    }

    [DebuggerDisplay("Value = {_value}")]
    public struct InplaceStringBuilderPatch
    {
        private int _offset;

        private int _capacity;

        private string _value;

        public int Capacity
        {
            get
            {
                return _capacity;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value)");
                }
                if (_offset > 0)
                {
                    throw new Exception("ThrowHelper.ThrowInvalidOperationException(ExceptionResource.Capacity_CannotChangeAfterWriteStarted)");
                }
                _capacity = value;
            }
        }

        public InplaceStringBuilderPatch(int capacity)
        {
            this = default(InplaceStringBuilderPatch);
            if (capacity < 0)
            {
                throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity)");
            }
            _capacity = capacity;
        }

        public void Append(string value)
        {
            if (value == null)
            {
                throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value)");
            }
            Append(value, 0, value.Length);
        }

        public void Append(StringSegment segment)
        {
            Append(segment.Buffer, segment.Offset, segment.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Append(string value, int offset, int count)
        {
            EnsureValueIsInitialized();
            if (value == null || offset < 0 || value.Length - offset < count || Capacity - _offset < count)
            {
                ThrowValidationError(value, offset, count);
            }
            fixed (char* ptr = _value)
            {
                fixed (char* ptr2 = value)
                {
                    Unsafe.CopyBlockUnaligned(ptr + _offset, ptr2 + offset, (uint)(count * 2));
                    _offset += count;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Append(char c)
        {
            EnsureValueIsInitialized();
            if (_offset >= Capacity)
            {
                throw new Exception("ThrowHelper.ThrowInvalidOperationException(ExceptionResource.Capacity_NotEnough, 1, Capacity - _offset)");
            }
            fixed (char* ptr = _value)
            {
                ptr[_offset++] = c;
            }
        }

        public override string ToString()
        {
            if (Capacity != _offset)
            {
                throw new Exception("ThrowHelper.ThrowInvalidOperationException(ExceptionResource.Capacity_NotUsedEntirely, Capacity, _offset)");
            }
            return _value;
        }

        private void EnsureValueIsInitialized()
        {
            if (_value == null)
            {
                _value = new string('\0', _capacity);
            }
        }

        private void ThrowValidationError(string value, int offset, int count)
        {
            if (value == null)
            {
                throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value)");
            }
            if (offset < 0 || value.Length - offset < count)
            {
                throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.offset)");
            }
            if (Capacity - _offset < count)
            {
                throw new Exception("ThrowHelper.ThrowInvalidOperationException(ExceptionResource.Capacity_NotEnough, value.Length, Capacity - _offset)");
            }
        }
    }

}
