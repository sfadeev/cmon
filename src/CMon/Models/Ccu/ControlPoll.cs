using System.Linq;

namespace CMon.Models.Ccu
{
	public class ControlPoll
	{
		public ControlPollSignal Signal { get; set; }

		public int ModemStatus { get; set; }

		public float Balance { get; set; }

		public int CellMode { get; set; }

		public float[,] In { get; set; }

		public int[] Out { get; set; }

		public ProfileInput[] ConvertInArray()
		{
			var length = In.GetLength(0);

			var result = new ProfileInput[length];

			for (var i = 0; i < length; i++)
			{
				result[i] = new ControlPollInput
				{
					InputType = (InputType)In[i, 0],
					MaxVoltage = (int)In[i, 1],
					UserMinVal = In[i, 2],
					UserMaxVal = In[i, 3],
					RangeType = (int)In[i, 4],
					Voltage = (int)In[i, 5]
				};
			}

			return result;
		}
	}
}