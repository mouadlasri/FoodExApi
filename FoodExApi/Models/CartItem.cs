using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodExApi.Models
{
    public class CartItem
    {
        public int? ItemId { get; set; }
        public string ItemImage { get; set; }
        public string ItemName { get; set; }
        public string ItemPrice { get; set; }
        public int? Quantity { get; set; }

    }
}
