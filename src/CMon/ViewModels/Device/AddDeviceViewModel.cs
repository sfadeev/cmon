using System.ComponentModel.DataAnnotations;

namespace CMon.ViewModels.Device
{
	public class AddDeviceViewModel
	{
		[Required]
		[StringLength(64)]
		[Display(Name = "Name")]
		public string Name { get; set; }

		[Required]
		[StringLength(15, MinimumLength = 15)]
		[Display(Name = "IMEI")]
		public string Imei { get; set; }

		[Required]
		[StringLength(100)]
		[Display(Name = "Device username")]
		public string Username { get; set; }

		[Required]
		[StringLength(100)]
		[DataType(DataType.Password)]
		[Display(Name = "Device password")]
		public string Password { get; set; }
	}
}