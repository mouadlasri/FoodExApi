using System;
using System.Collections.Generic;

namespace FoodExApi.Models
{
    public partial class OrderDetails
    {
        public int OrderId { get; set; }
        public int? ItemId { get; set; }
        public int? Quantity { get; set; }
        public int OrderDetailsId { get; set; }

        public virtual Item Item { get; set; }
        public virtual AppOrder Order { get; set; }
    }
}
