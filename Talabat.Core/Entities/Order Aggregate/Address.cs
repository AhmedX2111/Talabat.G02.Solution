﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
	public class Address
	{
		// For Order
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
        public string Street { get; set; } = null!;
		public string City { get; set; } = null!;
		public string Country { get; set; } = null!;

    }
}
