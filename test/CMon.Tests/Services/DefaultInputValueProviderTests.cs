using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CMon.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Xunit;

namespace CMon.Tests.Services
{
	public class DefaultInputValueProviderTests
    {
		public IConfigurationRoot Configuration { get; set; }

		public DefaultInputValueProviderTests()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddUserSecrets(UserSecret.Id)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

        [Fact]
        public void Test1()
        {
			// arrange
	        var connectionString = new ConnectionStringOptions
	        {
		        DefaultConnection = Configuration.GetConnectionString("DefaultConnection")
	        };
	        var provider = new DefaultInputValueProvider(Options.Create(connectionString));

			var now = DateTime.UtcNow;

			// act
	        var stats = provider.GetValues(new InputValueRequest
	        {
		        DeviceId = 0,
		        BeginDate = "now-2y",
		        EndDate = "now"
	        });

			// assert
			Assert.NotNull(stats);
        }

		[Fact]
		public void MultiThreadTest()
	    {
			var timers = new List<Timer>();

		    var activeCount = 0;

		    for (var i = 0; i < 5000; i++)
		    {
				timers.Add(new Timer(state =>
				{
					try
					{
						Interlocked.Increment(ref activeCount);

						Thread.Sleep(TimeSpan.FromSeconds(5));
					}
					finally
					{
						Interlocked.Decrement(ref activeCount);

						Console.WriteLine(
							$"{state} - {activeCount}/{Process.GetCurrentProcess().Threads.Count}");
					}
				}, i, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(15)));
			}

		    while (true)
		    {
		    }
		}
    }
}
