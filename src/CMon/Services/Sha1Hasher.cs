using System.Text;
using Newtonsoft.Json;

namespace CMon.Services
{
	public class Sha1Hasher
	{
		public string ComputeHash(object inputs)
		{
			var serialized = JsonConvert.SerializeObject(inputs);

			var encoding = Encoding.UTF8;

			var bytes = encoding.GetBytes(serialized);

			var sha1 = System.Security.Cryptography.SHA1.Create();

			var hash = sha1.ComputeHash(bytes);

			return encoding.GetString(hash);
		}
	}
}