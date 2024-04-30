﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Identity
{
	public class ApplicationUser : IdentityUser
	{
		public string DisplayName { get; set; } = null!;
		public Address? Address { get; set; } = null; // Navigational Property [ONE]
	}
}