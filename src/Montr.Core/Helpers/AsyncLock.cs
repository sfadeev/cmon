using System;
using System.Threading;
using System.Threading.Tasks;

namespace Montr.Core.Helpers
{
	// https://blogs.msdn.microsoft.com/pfxteam/2012/02/12/building-async-coordination-primitives-part-6-asynclock/
	public class AsyncLock
	{
		private readonly AsyncSemaphore _semaphore;
		private readonly Task<Releaser> _releaser;

		public AsyncLock()
		{
			_semaphore = new AsyncSemaphore(1);
			_releaser = Task.FromResult(new Releaser(this));
		}

		public Task<Releaser> LockAsync()
		{
			var wait = _semaphore.WaitAsync();
			return wait.IsCompleted ?
				_releaser :
				wait.ContinueWith((_, state) => new Releaser((AsyncLock)state),
					this, CancellationToken.None,
					TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}

		public struct Releaser : IDisposable
		{
			private readonly AsyncLock _toRelease;

			internal Releaser(AsyncLock toRelease)
			{
				_toRelease = toRelease;
			}

			public void Dispose()
			{
				_toRelease?._semaphore.Release();
			}
		}
	}
}