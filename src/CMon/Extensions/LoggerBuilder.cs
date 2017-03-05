using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace CMon.Extensions
{
    public static class LoggerBuilder
    {
	    public static ILogger Build(IHostingEnvironment env, string application)
	    {
			// Serilog.Debugging.SelfLog.Enable(Console.Error);

		    var pathFormat = (env.IsDevelopment() ? "../../../.logs/" : "/var/log/cmon/") + application + "-{Date}.log";

			var outputTemplate = 
				"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u4} [{ThreadId}] {SourceContext:l} - {Message}{NewLine}{Exception}";

		    var loggingLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Debug);

		    return new LoggerConfiguration()
				.MinimumLevel.ControlledBy(loggingLevelSwitch)
				.MinimumLevel.Override("System", LogEventLevel.Information)
				.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
				.WriteTo.LiterateConsole(outputTemplate: outputTemplate)
				.WriteTo.Async(a => a.RollingFile(pathFormat, outputTemplate: outputTemplate))
				.Enrich.FromLogContext()
				.Enrich.WithThreadId()
				.Enrich.WithProperty("Application", application)
				.CreateLogger();
		}
	}
}
