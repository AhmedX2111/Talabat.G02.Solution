using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
	public class DeliveryMethod : BaseEntity
	{
        public string ShortName { get; set; } = null!;
        public string ShortDescription { get; set; } = null!;
        public decimal Cost { get; set; }            
        public string DeliveryTime { get; set; } = null!; // Simple text to say the time to deliver the item

    }
}
