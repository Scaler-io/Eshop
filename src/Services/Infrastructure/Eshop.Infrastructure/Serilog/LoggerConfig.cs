using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;

namespace Eshop.Infrastructure.Serilog
{
    public class LoggerConfig
    {
        public static ILogger Configure(IConfiguration config)
        {

            var loggerOptions = new LoggerConfigOption();
            config.GetSection("LoggerConfigOption").Bind(loggerOptions);

            return new LoggerConfiguration()
                         .MinimumLevel.ControlledBy(new LoggingLevelSwitch(LogEventLevel.Debug))
                         .MinimumLevel.Override(loggerOptions.OverrideSource, LogEventLevel.Warning)
                         .WriteTo.Console(outputTemplate: loggerOptions.OutputTemplate)
                         .Enrich.FromLogContext()
                         .Enrich.WithProperty(nameof(Environment.MachineName), Environment.MachineName)
                         .CreateLogger();
        }
    }
}
