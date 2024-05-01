using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Infrastructure._Identity
{
	public static class ApplicationIdentityContextSeed
	{
		public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
		{

			if (!userManager.Users.Any())
			{
				var user = new ApplicationUser()
				{
					DisplayName = "Ahmed Khaled",
					Email = "AhmedKhaled910000@gmail.com",
					UserName = "Ahmed.Khaled",
					PhoneNumber = "01024005148"
				};

				await userManager.CreateAsync(user, "Pa$$w0rd");
			}
		}
	}
}
