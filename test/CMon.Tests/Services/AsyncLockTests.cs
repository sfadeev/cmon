using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using CMon.Services;
using Xunit;
using Xunit.Abstractions;

namespace CMon.Tests.Services
{
	public class AsyncLockTests
	{
		private readonly ITestOutputHelper _output;

		public AsyncLockTests(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void Test()
		{
			var clients = new ConcurrentDictionary<int, int>();
			var locks = new ConcurrentDictionary<int, AsyncLock>();

			Parallel.For(1, 100, async i =>
			{
				var client = i % 5;

				var @lock = locks.GetOrAdd(client, x => new AsyncLock());

				_output.WriteLine("Getting lock for client " + client);

				using (await @lock.LockAsync())
				{
					_output.WriteLine("Aquired lock for client " + client);

					Assert.Equal(-1, clients.GetOrAdd(client, x => -1));

					Assert.True(clients.TryUpdate(client, client, -1));

					await Task.Delay(TimeSpan.FromSeconds(1));

					Assert.True(clients.TryUpdate(client, -1, client));

					_output.WriteLine("Releasing lock for client " + client);
				}
			});
		}
	}
}