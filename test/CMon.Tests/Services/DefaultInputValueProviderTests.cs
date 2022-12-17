using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CMon.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace CMon.Tests.Services
{
	[TestFixture]
	public class DefaultInputValueProviderTests
    {
		public IConfigurationRoot Configuration { get; }

		public DefaultInputValueProviderTests()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddUserSecrets(typeof(AppSettings).Assembly)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

        [Test]
        public void Test1()
        {
			// arrange
			var provider = new DefaultInputValueProvider(NullLogger<DefaultInputValueProvider>.Instance, Configuration);

			// act
	        var stats = provider.GetValues(new InputValueRequest
	        {
		        DeviceId = 0,
		        BeginDate = "now-2y",
		        EndDate = "now"
	        }, new CancellationToken());

			// assert
			Assert.NotNull(stats);
        }

		[Test]
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
