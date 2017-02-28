using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;

namespace CMon.Services
{
    public interface IBackgroundJob
    {
	    string Enqueue<T>(Expression<Func<T, Task>> methodCall);
    }

	public class HangfireBackgroundJob : IBackgroundJob
	{
		public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
		{
			return BackgroundJob.Enqueue(methodCall);
		}
	}
}
