using System.Threading.Tasks;

namespace CMon.Web.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }

	public class DefaultSmsSender : ISmsSender
	{
		public Task SendSmsAsync(string number, string message)
		{
			return Task.FromResult(0);
		}
	}
}
