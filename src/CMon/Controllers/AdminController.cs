using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMon.Entities;
using CMon.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CMon.Controllers
{
	[Authorize(Roles = "Administrator")]
	public class AdminController: Controller
	{
		private readonly IOptions<ConnectionStringsOptions> _options;
		private readonly IRoleStore<DbRole> _roleStore;

		public AdminController(IOptions<ConnectionStringsOptions> options, IRoleStore<DbRole> roleStore)
		{
			_options = options;
			_roleStore = roleStore;
		}

		public async Task<IActionResult> Index()
		{
			var model = new StatusViewModel
			{
				DatabaseName = _options.Value.Default
			};

			/*var role = await _roleStore.FindByNameAsync("administrator", new CancellationToken());

			if (role == null)
			{
				var dbRole = new DbRole
				{
					Name = "Administrator",
					NormalizedName = "administrator"
				};

				var result = await _roleStore.CreateAsync(dbRole, new CancellationToken());

				if (result.Succeeded == false)
				{
					throw new Exception(string.Concat(result.Errors));
				}
			}*/

			return View(model);
		}
	}
}
