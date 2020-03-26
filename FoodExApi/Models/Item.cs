using System;
using System.Collections.Generic;

namespace FoodExApi.Models
{
    public partial class Item
    {
        public Item()
        {
            OrderDetails = new HashSet<OrderDetails>();
        }

        public int ItemId { get; set; }
        public int? RestaurantId { get; set; }
        public string Name { get; set; }
        public int? Category { get; set; }
        public decimal? Price { get; set; }

        public virtual Restaurant Restaurant { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
