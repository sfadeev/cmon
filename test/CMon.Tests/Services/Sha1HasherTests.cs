using System.Linq;
using CMon.Models.Ccu;
using CMon.Services;
using Xunit;

namespace CMon.Tests.Services
{
	public class Sha1HasherTests
	{
		[Fact]
		public void Sha1Hasher_WithCcuConfiguration_ShouldComputeHash()
		{
			// arrange
			var hasher = new Sha1Hasher();

			var inputs = Enumerable.Range(0, 16)
				.Select(
					x => new InputsInputNum
					{
						InputNum = (short) x,
						InputName = "INPUT_" + x
					}).ToList();

			// act
			var result = hasher.Compute(inputs);

			// assert
			Assert.NotNull(result);
		}
	}
}