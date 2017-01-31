using System;
using System.Collections.Generic;

namespace CMon.Models
{
	public class InputValue
	{
		public short InputNum { get; set; }

		public decimal Value { get; set; }

		public string Date { get; set; }
	}

	public class InputPeriodValue
	{
		public DateTime Period { get; set; }

		public decimal Avg { get; set; }

		public decimal Min { get; set; }

		public decimal Max { get; set; }
	}

	public class InputStatistic
	{
		public short InputNo { get; set; }

		public string Name { get; set; }

		public IList<InputPeriodValue> Values { get; set; }
	}

	public class DeviceStatistic
	{
		public DateTime BeginDate { get; set; }

		public DateTime EndDate { get; set; }

		public IList<InputStatistic> Inputs { get; set; }
	}
}