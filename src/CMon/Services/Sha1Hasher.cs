using System.Text;
using Newtonsoft.Json;

namespace CMon.Services
{
	public class Sha1Hasher
	{
		public byte[] Compute(object value)
		{
			var serialized = JsonConvert.SerializeObject(value);

			var bytes = Encoding.UTF8.GetBytes(serialized);

			var sha1 = System.Security.Cryptography.SHA1.Create();

			return sha1.ComputeHash(bytes);
		}
	}
}